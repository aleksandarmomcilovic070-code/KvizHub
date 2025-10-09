import React from "react";
import { Link } from "react-router-dom";
import { getUserRole, logout, isTokenValid } from "../utils/auth";

function Header() {
  const role = getUserRole();
  const loggedIn = isTokenValid();

  return (
    <header style={{ display: "flex", gap: "1rem", marginBottom: "2rem" }}>
      <Link to="/">Home</Link>
      {!loggedIn && <Link to="/login">Login</Link>}
      {!loggedIn && <Link to="/register">Register</Link>}
      {loggedIn && role === "Admin" && <Link to="/admin">Admin panel</Link>}
      {loggedIn && role === "Player" && <Link to="/player">Player panel</Link>}
      {loggedIn && (
        <button onClick={logout} style={{ marginLeft: "auto" }}>
          Logout
        </button>
      )}
    </header>
  );
}

export default Header;
