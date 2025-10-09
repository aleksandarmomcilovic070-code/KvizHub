import React from "react";
import { Navigate } from "react-router-dom";
import { isTokenValid, getUserRole } from "../utils/auth";

const ProtectedRoute = ({ element, requiredRole }) => {
  const valid = isTokenValid();
  const role = getUserRole();

  if (!valid) return <Navigate to="/login" />;
  if (requiredRole && role !== requiredRole) return <Navigate to="/" />;

  return element;
};

export default ProtectedRoute;
