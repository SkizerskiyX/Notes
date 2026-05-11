const AUTH_KEY = "notesproject.auth";

export function readStoredAuth() {
  try {
    const raw = localStorage.getItem(AUTH_KEY);
    if (!raw) return null;
    const parsed = JSON.parse(raw);
    if (!parsed || typeof parsed.accessToken !== "string" || typeof parsed.refreshToken !== "string") return null;
    return parsed;
  } catch {
    return null;
  }
}

/** @param {{ accessToken: string; refreshToken: string; accessTokenExpiresAtUtc: string; email?: string }} data */
export function writeStoredAuth(data) {
  localStorage.setItem(AUTH_KEY, JSON.stringify(data));
}

export function clearStoredAuth() {
  localStorage.removeItem(AUTH_KEY);
}

/** @param {number} bufferSec skew before expiry */
export function isAccessTokenFresh(bufferSec = 45) {
  const auth = readStoredAuth();
  if (!auth?.accessTokenExpiresAtUtc) return false;
  const exp = Date.parse(auth.accessTokenExpiresAtUtc);
  if (Number.isNaN(exp)) return false;
  return Date.now() < exp - bufferSec * 1000;
}

export function hasRefreshSession() {
  return Boolean(readStoredAuth()?.refreshToken);
}
