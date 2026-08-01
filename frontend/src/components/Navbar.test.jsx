import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import { AuthContext } from '../contexts/AuthContext';
import Navbar from './Navbar';

const mockUser = { username: 'admin' };
const mockLogout = vi.fn();

const renderNavbar = () => {
  return render(
    <AuthContext.Provider value={{ user: mockUser, logout: mockLogout }}>
      <BrowserRouter>
        <Navbar />
      </BrowserRouter>
    </AuthContext.Provider>
  );
};

describe('Navbar Component Unit Tests', () => {
  it('renders brand name and search bar', () => {
    renderNavbar();
    expect(screen.getByText('LICENTRA')).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/Search Application or People Here/i)).toBeInTheDocument();
  });

  it('handles font size scaling buttons (A-, A, A+)', () => {
    renderNavbar();
    const btnSmall = screen.getByText('A-');
    const btnNormal = screen.getByText('A');
    const btnLarge = screen.getByText('A+');

    fireEvent.click(btnSmall);
    expect(document.documentElement.style.fontSize).toBe('14px');

    fireEvent.click(btnLarge);
    expect(document.documentElement.style.fontSize).toBe('18px');

    fireEvent.click(btnNormal);
    expect(document.documentElement.style.fontSize).toBe('16px');
  });

  it('toggles dark mode theme when theme button is clicked', () => {
    renderNavbar();
    const themeBtn = screen.getByText('🌙');

    fireEvent.click(themeBtn);
    expect(document.body.classList.contains('dark-theme')).toBe(true);

    const sunBtn = screen.getByText('☀️');
    fireEvent.click(sunBtn);
    expect(document.body.classList.contains('dark-theme')).toBe(false);
  });

  it('opens and filters dropdown when typing in search bar', () => {
    renderNavbar();
    const searchInput = screen.getByPlaceholderText(/Search Application or People Here/i);

    fireEvent.change(searchInput, { target: { value: 'Adobe' } });
    expect(screen.getByText('Adobe Creative Cloud')).toBeInTheDocument();
  });

  it('opens About/Profile popover when Admin badge is clicked', () => {
    renderNavbar();
    const adminBadge = screen.getByText('AD');

    fireEvent.click(adminBadge);
    expect(screen.getByText('System Administrator')).toBeInTheDocument();
    expect(screen.getByText(/v2.4.0/i)).toBeInTheDocument();
  });

  it('triggers logout function when Logout button is clicked', () => {
    renderNavbar();
    const logoutBtn = screen.getByTitle('Logout of Licentra');

    fireEvent.click(logoutBtn);
    expect(mockLogout).toHaveBeenCalledTimes(1);
  });
});
