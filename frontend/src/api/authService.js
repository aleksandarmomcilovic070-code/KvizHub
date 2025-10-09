import axios from "axios";
import { LoginModel } from "../models/loginModel";
import { RegisterModel } from "../models/registerModel";

const API_URL = process.env.REACT_APP_API_BASE_URL;

export const login = async (loginData) => {
  try {
    if (!(loginData instanceof LoginModel)) {
      throw new Error("Invalid login data model");
    }

    const response = await axios.post(`${API_URL}/User/Login`, {
      username: loginData.username,
      password: loginData.password,
    });

    return response.data; // token
  } catch (error) {
    console.error("Login error:", error);
    throw error;
  }
};

export const register = async (registerData) => {
  try {
    if (!(registerData instanceof RegisterModel)) {
      throw new Error("Invalid register data model");
    }

    const response = await axios.post(`${API_URL}/User/Register`, {
      username: registerData.username,
      email: registerData.email,
      password: registerData.password,
    });

    return response.data;
  } catch (error) {
    console.error("Registration error:", error);
    throw error;
  }
};