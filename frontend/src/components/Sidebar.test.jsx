import React from 'react';
import { render, screen } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import Sidebar from './Sidebar';

describe('Sidebar Navigation Unit Tests', () => {
  it('renders all navigation items with correct links', () => {
    render(
      <BrowserRouter>
        <Sidebar />
      </BrowserRouter>
    );

    const expectedNavs = [
      'Dashboard', 'Licenses', 'Assignments', 'Employees', 
      'Software', 'Vendors', 'Departments', 'Roles', 'Users', 'Audit Logs'
    ];

    expectedNavs.forEach((navText) => {
      expect(screen.getByText(navText)).toBeInTheDocument();
    });
  });
});
