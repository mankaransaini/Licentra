import React from 'react';
import { render, screen } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import CrudPage from './CrudPage';

// Mock anime.js to prevent animation errors during tests
vi.mock('animejs', () => ({
  default: () => ({}),
}));

describe('CrudPage Component', () => {
  it('renders the title correctly', () => {
    const columns = [
      { key: 'id', label: 'ID' },
      { key: 'name', label: 'Name' },
    ];
    
    render(<CrudPage title="Test Departments" endpoint="/test" columns={columns} />);
    
    expect(screen.getByText('Test Departments')).toBeInTheDocument();
  });

  it('renders column headers correctly', () => {
    const columns = [
      { key: 'id', label: 'ID' },
      { key: 'name', label: 'Name' },
    ];
    
    render(<CrudPage title="Test" endpoint="/test" columns={columns} />);
    
    expect(screen.getAllByText('ID').length).toBeGreaterThan(0);
    expect(screen.getAllByText('Name').length).toBeGreaterThan(0);
    expect(screen.getByText('Actions')).toBeInTheDocument();
  });
  
  it('shows loading state initially', () => {
    const columns = [{ key: 'id', label: 'ID' }];
    render(<CrudPage title="Test" endpoint="/test" columns={columns} />);
    
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });
});
