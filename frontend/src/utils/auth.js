import { jwtDecode } from "jwt-decode";

export const getToken = () => localStorage.getItem("token");

export const getUserRole = () => {
  const token = getToken();
  console.log(token);
  if (!token) return null;
  try {
    const decoded = jwtDecode(token);
    console.log(decoded);
    return decoded.role || null;
  } catch {
    return null;
  }
};

export const isTokenValid = () => {
  const token = getToken();
  if (!token) return false;

  try {
    const decoded = jwtDecode(token);
    const now = Date.now() / 1000; // u sekundama
    return decoded.exp && decoded.exp > now;
  } catch {
    return false;
  }
};

export const logout = () => {
  localStorage.removeItem("token");
  window.location.href = "/login";
};
