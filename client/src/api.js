import axios from "axios";
import { clearStoredAuth, isAccessTokenFresh, readStoredAuth, writeStoredAuth } from "./authStorage";

const api = axios.create({
  baseURL: "http://localhost:5262/api"
});

let refreshPromise = null;

function attachAuth(config) {
  const auth = readStoredAuth();
  if (auth?.accessToken) {
    config.headers.Authorization = `Bearer ${auth.accessToken}`;
  }
  return config;
}

api.interceptors.request.use((config) => attachAuth(config));

async function refreshAccessToken() {
  const stored = readStoredAuth();
  if (!stored?.refreshToken) throw new Error("Missing refresh token");

  const response = await axios.post(
    `${api.defaults.baseURL}/auth/refresh`,
    { refreshToken: stored.refreshToken },
    { headers: { "Content-Type": "application/json" } }
  );

  const data = response.data;
  writeStoredAuth({
    accessToken: data.accessToken ?? data.AccessToken,
    refreshToken: data.refreshToken ?? data.RefreshToken,
    accessTokenExpiresAtUtc: data.accessTokenExpiresAtUtc ?? data.AccessTokenExpiresAtUtc,
    email: stored.email
  });
}

/** Returns whether the browser has a usable session (uses refresh token when access token expired). */
export async function ensureValidSession() {
  const stored = readStoredAuth();
  if (!stored?.refreshToken || !stored.accessToken) return false;
  if (isAccessTokenFresh()) return true;
  try {
    await refreshAccessToken();
    return true;
  } catch {
    clearStoredAuth();
    return false;
  }
}

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const cfg = error.config;
    const status = error.response?.status;
    const url = cfg?.url ?? "";

    const isRefreshCall = typeof url === "string" && url.includes("/auth/refresh");
    if (!cfg || cfg._authRetry || status !== 401 || isRefreshCall) {
      return Promise.reject(error);
    }

    if (!refreshPromise) {
      refreshPromise = refreshAccessToken().finally(() => {
        refreshPromise = null;
      });
    }

    try {
      await refreshPromise;
      const next = readStoredAuth();
      cfg._authRetry = true;
      cfg.headers.Authorization = next?.accessToken ? `Bearer ${next.accessToken}` : undefined;
      return api(cfg);
    } catch {
      clearStoredAuth();
      return Promise.reject(error);
    }
  }
);

export async function loginRequest(payload) {
  const response = await api.post("/auth/login", payload);
  return response.data;
}

export async function registerRequest(payload) {
  const response = await api.post("/auth/register", payload);
  return response.data;
}

export async function revokeRefreshRequest(refreshToken) {
  try {
    await api.post("/auth/revoke", { refreshToken });
  } catch {
    /* ignore */
  }
}

export async function getNotes() {
  const response = await api.get("/note");
  return response.data;
}

export async function createNote(payload) {
  const response = await api.post("/note", payload);
  return response.data;
}

export async function updateNote(id, payload) {
  const response = await api.put(`/note/${id}`, payload);
  return response.data;
}

export async function deleteNote(id) {
  await api.delete(`/note/${id}`);
}

export default api;
