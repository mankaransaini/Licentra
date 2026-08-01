import React, { createContext, useState, useEffect } from 'react';
import api from '../services/api';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const token = localStorage.getItem('token');
    const storedUser = localStorage.getItem('user');
    if (token) {
      let userData = { token };
      if (storedUser) {
        try {
          userData = { token, ...JSON.parse(storedUser) };
        } catch (e) {
          console.error("Failed to parse stored user", e);
        }
      }
      setUser(userData);
    }
    setLoading(false);
  }, []);

  const login = async (username, password) => {
    try {
      setError('');
      const response = await api.post('/auth/login', { username, password });
      const authData = response.data; // Unwrapped by api response interceptor

      if (authData && authData.token) {
        localStorage.setItem('token', authData.token);
        const userInfo = { username: authData.username || username, role: authData.role };
        localStorage.setItem('user', JSON.stringify(userInfo));
        setUser({ token: authData.token, ...userInfo });
        return true;
      } else {
        setError('Invalid response from server');
        return false;
      }
    } catch (err) {
      const serverMsg = err.response?.data?.message || (typeof err.response?.data === 'string' ? err.response.data : null);
      setError(serverMsg || 'Login failed. Please check your username and password.');
      return false;
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading, error }}>
      {children}
    </AuthContext.Provider>
  );
};
