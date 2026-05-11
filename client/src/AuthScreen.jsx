import { useState } from "react";
import { loginRequest, registerRequest } from "./api";
import { writeStoredAuth } from "./authStorage";

export default function AuthScreen({ theme, onToggleTheme, onSignedIn }) {
  const [mode, setMode] = useState("login");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");

  async function persistSession(data, normalizedEmail) {
    const accessToken = data.accessToken ?? data.AccessToken ?? "";
    const refreshToken = data.refreshToken ?? data.RefreshToken ?? "";
    const accessTokenExpiresAtUtc = data.accessTokenExpiresAtUtc ?? data.AccessTokenExpiresAtUtc ?? "";
    writeStoredAuth({
      accessToken,
      refreshToken,
      accessTokenExpiresAtUtc,
      email: normalizedEmail
    });
    onSignedIn();
  }

  async function handleLogin(event) {
    event.preventDefault();
    setError("");
    const trimmed = email.trim().toLowerCase();
    if (!trimmed || !password) return;
    try {
      setBusy(true);
      const data = await loginRequest({ email: trimmed, password });
      await persistSession(data, trimmed);
      setPassword("");
    } catch (e) {
      const data = e.response?.data;
      const msg = typeof data?.message === "string" ? data.message : typeof data?.Message === "string" ? data.Message : null;
      setError(msg ?? "Could not sign in. Check your email and password.");
    } finally {
      setBusy(false);
    }
  }

  async function handleRegister(event) {
    event.preventDefault();
    setError("");
    const trimmed = email.trim().toLowerCase();
    if (!trimmed || !password) return;
    if (password !== confirm) {
      setError("Passwords do not match.");
      return;
    }
    try {
      setBusy(true);
      const data = await registerRequest({ email: trimmed, password });
      await persistSession(data, trimmed);
      setPassword("");
      setConfirm("");
    } catch (e) {
      const data = e.response?.data;
      const msg = typeof data?.message === "string" ? data.message : typeof data?.Message === "string" ? data.Message : null;
      setError(msg ?? "Could not create account. Try another email.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="auth-screen">
      <div className="auth-backdrop" aria-hidden />

      <div className="auth-card card">
        <header className="auth-card-head">
          <div>
            <p className="auth-eyebrow">Notes Project</p>
            <h1 className="auth-title">{mode === "login" ? "Welcome back" : "Create your account"}</h1>
            <p className="auth-subtitle">
              {mode === "login"
                ? "Sign in to sync your notes with your account."
                : "Save your workspace and pick up where you left off."}
            </p>
          </div>
          <button type="button" className="theme-toggle auth-theme" onClick={onToggleTheme}>
            {theme === "dark" ? "Light mode" : "Dark mode"}
          </button>
        </header>

        <div className="auth-tabs tab-row">
          <button
            type="button"
            className={`tab-btn ${mode === "login" ? "active" : ""}`}
            onClick={() => {
              setMode("login");
              setError("");
            }}
          >
            Sign in
          </button>
          <button
            type="button"
            className={`tab-btn ${mode === "register" ? "active" : ""}`}
            onClick={() => {
              setMode("register");
              setError("");
            }}
          >
            Register
          </button>
        </div>

        {mode === "login" ? (
          <form className="auth-form" onSubmit={handleLogin}>
            <label className="auth-label">
              <span>Email</span>
              <input
                className="text-input"
                type="email"
                autoComplete="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="you@example.com"
                required
              />
            </label>
            <label className="auth-label">
              <span>Password</span>
              <input
                className="text-input"
                type="password"
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Your password"
                required
              />
            </label>
            {error ? <p className="error">{error}</p> : null}
            <button type="submit" className="auth-submit" disabled={busy}>
              {busy ? "Signing in..." : "Sign in"}
            </button>
          </form>
        ) : (
          <form className="auth-form" onSubmit={handleRegister}>
            <label className="auth-label">
              <span>Email</span>
              <input
                className="text-input"
                type="email"
                autoComplete="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="you@example.com"
                required
              />
            </label>
            <label className="auth-label">
              <span>Password</span>
              <input
                className="text-input"
                type="password"
                autoComplete="new-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Choose a strong password"
                required
              />
            </label>
            <label className="auth-label">
              <span>Confirm password</span>
              <input
                className="text-input"
                type="password"
                autoComplete="new-password"
                value={confirm}
                onChange={(e) => setConfirm(e.target.value)}
                placeholder="Repeat password"
                required
              />
            </label>
            {error ? <p className="error">{error}</p> : null}
            <button type="submit" className="auth-submit" disabled={busy}>
              {busy ? "Creating account..." : "Create account"}
            </button>
          </form>
        )}
      </div>
    </div>
  );
}
