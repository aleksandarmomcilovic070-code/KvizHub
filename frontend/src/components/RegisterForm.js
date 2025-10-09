import React from "react";
import { useFormik } from "formik";
import { register } from "../api/authService";
import { RegisterModel } from "../models/registerModel";
import { useNavigate } from "react-router-dom";

function RegisterForm() {
  const navigate = useNavigate();

  const formik = useFormik({
    initialValues: {
      username: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
    validate: (values) => {
      const errors = {};
      if (!values.username) errors.username = "Obavezno polje";
      if (!values.email) errors.email = "Obavezno polje";
      else if (!/\S+@\S+\.\S+/.test(values.email)) errors.email = "Nevalidan email";
      if (values.password.length < 6)
        errors.password = "Lozinka mora imati najmanje 6 karaktera";
      if (values.password !== values.confirmPassword)
        errors.confirmPassword = "Lozinke se ne poklapaju";
      return errors;
    },
    onSubmit: async (values) => {
      try {
        const payload = new RegisterModel(values.username, values.email, values.password);
        await register(payload);
        alert("Uspešna registracija!");
        navigate("/login");
      } catch (error) {
        alert("Greška pri registraciji!");
      }
    },
  });

  return (
    <form onSubmit={formik.handleSubmit}>
      <h2>Registracija</h2>

      <label>Username</label>
      <input
        name="username"
        type="text"
        onChange={formik.handleChange}
        value={formik.values.username}
      />
      {formik.errors.username && <div>{formik.errors.username}</div>}

      <label>Email</label>
      <input
        name="email"
        type="email"
        onChange={formik.handleChange}
        value={formik.values.email}
      />
      {formik.errors.email && <div>{formik.errors.email}</div>}

      <label>Password</label>
      <input
        name="password"
        type="password"
        onChange={formik.handleChange}
        value={formik.values.password}
      />
      {formik.errors.password && <div>{formik.errors.password}</div>}

      <label>Confirm Password</label>
      <input
        name="confirmPassword"
        type="password"
        onChange={formik.handleChange}
        value={formik.values.confirmPassword}
      />
      {formik.errors.confirmPassword && <div>{formik.errors.confirmPassword}</div>}

      <button type="submit">Registruj se</button>
    </form>
  );
}

export default RegisterForm;
