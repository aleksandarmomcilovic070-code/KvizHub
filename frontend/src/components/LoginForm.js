import React from "react";
import { useFormik } from "formik";
import { login } from "../api/authService";
import { LoginModel } from "../models/loginModel";
import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";

function LoginForm() {
  const navigate = useNavigate();

  const formik = useFormik({
    initialValues: { username: "", password: "" },
    onSubmit: async (values) => {
      try {
        const payload = new LoginModel(values.username, values.password);
        const token = await login(payload);

        localStorage.setItem("token", token);

        const decoded = jwtDecode(token);
        const role = decoded.role;

        if (role === "Admin") navigate("/admin");
        else if (role === "Player") navigate("/player");
        else navigate("/");

      } catch (error) {
        alert("Greška pri prijavi!");
      }
    },
  });

  return (
    <form onSubmit={formik.handleSubmit}>
      <h2>Login</h2>
      <label>Username</label>
      <input
        name="username"
        type="text"
        onChange={formik.handleChange}
        value={formik.values.username}
      />
      <label>Password</label>
      <input
        name="password"
        type="password"
        onChange={formik.handleChange}
        value={formik.values.password}
      />
      <button type="submit">Prijavi se</button>
    </form>
  );
}

export default LoginForm;
