import { useEffect, useMemo, useState } from "react";
import AuthScreen from "./AuthScreen";
import {
  clearStoredAuth,
  readStoredAuth
} from "./authStorage";
import { createNote, deleteNote, ensureValidSession, getNotes, revokeRefreshRequest, updateNote } from "./api";

const EMPTY_FORM = {
  header: "",
  text: "",
  isPinned: false
};

export default function App() {
  const [session, setSession] = useState("checking");
  const [userEmail, setUserEmail] = useState("");
  const [notes, setNotes] = useState([]);
  const [form, setForm] = useState(EMPTY_FORM);
  const [editForm, setEditForm] = useState(EMPTY_FORM);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [updating, setUpdating] = useState(false);
  const [error, setError] = useState("");
  const [search, setSearch] = useState("");
  const [selectedId, setSelectedId] = useState(null);
  const [activeTab, setActiveTab] = useState("view");
  const [theme, setTheme] = useState("dark");

  const filteredNotes = useMemo(() => {
    const term = search.trim().toLowerCase();
    const visible = notes.filter((note) => {
      if (!term) return true;
      return (
        note.header.toLowerCase().includes(term) ||
        note.text.toLowerCase().includes(term)
      );
    });

    return visible.sort((a, b) => Number(b.isPinned) - Number(a.isPinned));
  }, [notes, search]);

  const selectedNote = useMemo(
    () => notes.find((note) => note.id === selectedId) ?? null,
    [notes, selectedId]
  );

  useEffect(() => {
    void (async () => {
      const ok = await ensureValidSession();
      if (ok) {
        const auth = readStoredAuth();
        setUserEmail(auth?.email ?? "");
        setSession("in");
      } else {
        setSession("out");
      }
    })();
  }, []);

  useEffect(() => {
    if (session !== "in") return;
    void loadNotes();
  }, [session]);

  useEffect(() => {
    document.documentElement.setAttribute("data-theme", theme);
  }, [theme]);

  useEffect(() => {
    if (!notes.length) {
      setSelectedId(null);
      return;
    }

    if (!selectedId || !notes.some((note) => note.id === selectedId)) {
      setSelectedId(notes[0].id);
    }
  }, [notes, selectedId]);

  useEffect(() => {
    if (!selectedNote) {
      setEditForm(EMPTY_FORM);
      return;
    }

    setEditForm({
      header: selectedNote.header,
      text: selectedNote.text,
      isPinned: selectedNote.isPinned
    });
  }, [selectedNote]);

  async function loadNotes() {
    try {
      setLoading(true);
      setError("");
      const data = await getNotes();
      setNotes(data ?? []);
    } catch (requestError) {
      setError("Could not load notes. Make sure the API is running.");
    } finally {
      setLoading(false);
    }
  }

  async function handleLogout() {
    const rt = readStoredAuth()?.refreshToken;
    await revokeRefreshRequest(rt);
    clearStoredAuth();
    setNotes([]);
    setSelectedId(null);
    setUserEmail("");
    setSession("out");
  }

  function handleSignedInFromAuthScreen() {
    const auth = readStoredAuth();
    setUserEmail(auth?.email ?? "");
    setSession("in");
  }

  async function handleCreate(event) {
    event.preventDefault();
    if (!form.header.trim() || !form.text.trim()) return;

    try {
      setSaving(true);
      const created = await createNote({
        header: form.header.trim(),
        text: form.text.trim(),
        isPinned: form.isPinned
      });
      setNotes((prev) => [created, ...prev]);
      setSelectedId(created.id);
      setActiveTab("view");
      setForm(EMPTY_FORM);
    } catch (requestError) {
      setError("Could not create note.");
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(id) {
    const shouldDelete = window.confirm("Are you sure you want to delete this note?");
    if (!shouldDelete) return;

    try {
      await deleteNote(id);
      setNotes((prev) => {
        const remaining = prev.filter((note) => note.id !== id);
        if (selectedId === id) {
          setSelectedId(remaining[0]?.id ?? null);
          setActiveTab("view");
        }
        return remaining;
      });
    } catch (requestError) {
      setError("Could not delete note.");
    }
  }

  async function togglePin(note) {
    try {
      const updated = await updateNote(note.id, {
        header: note.header,
        text: note.text,
        isPinned: !note.isPinned
      });
      setNotes((prev) => prev.map((item) => (item.id === note.id ? updated : item)));
    } catch (requestError) {
      setError("Could not update pin state.");
    }
  }

  async function handleUpdate(event) {
    event.preventDefault();

    if (!selectedNote) return;
    if (!editForm.header.trim() || !editForm.text.trim()) return;

    try {
      setUpdating(true);
      const updated = await updateNote(selectedNote.id, {
        header: editForm.header.trim(),
        text: editForm.text.trim(),
        isPinned: editForm.isPinned
      });
      setNotes((prev) => prev.map((item) => (item.id === selectedNote.id ? updated : item)));
      setActiveTab("view");
    } catch (requestError) {
      setError("Could not update note.");
    } finally {
      setUpdating(false);
    }
  }

  if (session === "checking") {
    return (
      <div className="auth-screen auth-screen--minimal">
        <div className="auth-backdrop" aria-hidden />
        <div className="auth-loading card">
          <p className="auth-eyebrow">Notes Project</p>
          <p className="auth-loading-msg">Loading your workspace...</p>
        </div>
      </div>
    );
  }

  if (session === "out") {
    return (
      <AuthScreen
        theme={theme}
        onToggleTheme={() => setTheme((prev) => (prev === "dark" ? "light" : "dark"))}
        onSignedIn={handleSignedInFromAuthScreen}
      />
    );
  }

  return (
    <div className="shell">
      <aside className="sidebar-panel">
        <div className="sidebar-head">
          <h2>Notes</h2>
          <input
            className="search-input"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search notes..."
          />
        </div>
        <div className="note-list">
          {filteredNotes.map((note) => (
            <button
              key={note.id}
              className={`note-list-item ${selectedId === note.id ? "active" : ""}`}
              onClick={() => {
                setSelectedId(note.id);
                setActiveTab("view");
              }}
              type="button"
            >
              <div className="note-list-top">
                <h3>{note.header}</h3>
                {note.isPinned ? <span className="badge">Pinned</span> : null}
              </div>
              <p>{note.text}</p>
            </button>
          ))}
        </div>
      </aside>

      <main className="main-panel">
        <header className="main-head">
          <div className="main-head-brand">
            <h1>My Notes</h1>
            {userEmail ? <span className="user-chip">{userEmail}</span> : null}
          </div>
          <div className="main-head-actions">
            <button type="button" className="theme-toggle" onClick={() => setTheme((prev) => (prev === "dark" ? "light" : "dark"))}>
              {theme === "dark" ? "Light mode" : "Dark mode"}
            </button>
            <button type="button" className="ghost sign-out-btn" onClick={() => void handleLogout()}>
              Sign out
            </button>
          </div>
        </header>

        <section className="composer card">
          <form onSubmit={handleCreate}>
            <div className="input-row">
              <input
                className="text-input"
                value={form.header}
                onChange={(event) => setForm((prev) => ({ ...prev, header: event.target.value }))}
                placeholder="Note title"
                maxLength={80}
                required
              />
            </div>
            <textarea
              className="text-area"
              value={form.text}
              onChange={(event) => setForm((prev) => ({ ...prev, text: event.target.value }))}
              placeholder="Write your note..."
              required
            />
            <div className="composer-actions">
              <label className="checkbox">
                <input
                  type="checkbox"
                  checked={form.isPinned}
                  onChange={(event) => setForm((prev) => ({ ...prev, isPinned: event.target.checked }))}
                />
                Pin on create
              </label>
              <button type="submit" disabled={saving}>
                {saving ? "Saving..." : "Create note"}
              </button>
            </div>
          </form>
        </section>

        <section className="details-panel card">
          {!selectedNote ? (
            <p className="status">Select a note to view details.</p>
          ) : (
            <>
              <div className="detail-head">
                <h2>{selectedNote.header}</h2>
                <div className="tab-row">
                  <button
                    className={`tab-btn ${activeTab === "view" ? "active" : ""}`}
                    onClick={() => setActiveTab("view")}
                    type="button"
                  >
                    View
                  </button>
                  <button
                    className={`tab-btn ${activeTab === "edit" ? "active" : ""}`}
                    onClick={() => setActiveTab("edit")}
                    type="button"
                  >
                    Edit
                  </button>
                </div>
              </div>

              {activeTab === "view" ? (
                <div className="detail-body">
                  <article className="note-detail-content">{selectedNote.text}</article>
                  <div className="note-actions pinned-bottom">
                    <button className="ghost" onClick={() => togglePin(selectedNote)} type="button">
                      {selectedNote.isPinned ? "Unpin" : "Pin"}
                    </button>
                    <button className="danger" onClick={() => handleDelete(selectedNote.id)} type="button">
                      Delete
                    </button>
                  </div>
                </div>
              ) : (
                <form className="detail-body" onSubmit={handleUpdate}>
                  <input
                    className="text-input"
                    value={editForm.header}
                    onChange={(event) => setEditForm((prev) => ({ ...prev, header: event.target.value }))}
                    placeholder="Note title"
                    maxLength={80}
                    required
                  />
                  <textarea
                    className="text-area detail-edit-area"
                    value={editForm.text}
                    onChange={(event) => setEditForm((prev) => ({ ...prev, text: event.target.value }))}
                    placeholder="Edit note..."
                    required
                  />
                  <label className="checkbox">
                    <input
                      type="checkbox"
                      checked={editForm.isPinned}
                      onChange={(event) =>
                        setEditForm((prev) => ({ ...prev, isPinned: event.target.checked }))
                      }
                    />
                    Keep pinned
                  </label>
                  <div className="note-actions pinned-bottom">
                    <button className="ghost" type="button" onClick={() => setActiveTab("view")}>
                      Cancel
                    </button>
                    <button type="submit" disabled={updating}>
                      {updating ? "Saving..." : "Save changes"}
                    </button>
                  </div>
                </form>
              )}
            </>
          )}
          {error ? <p className="error">{error}</p> : null}
          {loading ? <p className="status">Loading notes...</p> : null}
        </section>
      </main>
    </div>
  );
}
