import React from 'react';
import { render, screen } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import Dashboard from './Dashboard';

vi.mock('animejs', () => {
  const fn = () => ({});
  fn.stagger = () => () => {};
  return { default: fn };
});

vi.mock('recharts', () => ({
  ResponsiveContainer: ({ children }) => <div>{children}</div>,
  BarChart: ({ children }) => <div data-testid="bar-chart">{children}</div>,
  Bar: () => null,
  XAxis: () => null,
  YAxis: () => null,
  CartesianGrid: () => null,
  Tooltip: () => null,
  AreaChart: ({ children }) => <div data-testid="area-chart">{children}</div>,
  Area: () => null,
}));

describe('Dashboard Component Unit Tests', () => {
  it('renders hero banner and stats cards correctly', () => {
    render(
      <BrowserRouter>
        <Dashboard />
      </BrowserRouter>
    );

    expect(screen.getByText(/Manage Software/i)).toBeInTheDocument();
    expect(screen.getByText('SOFTWARE CATALOG')).toBeInTheDocument();
    expect(screen.getByText('TOTAL LICENSES')).toBeInTheDocument();
    expect(screen.getByText('SEAT ALLOCATION')).toBeInTheDocument();
    expect(screen.getByText('COMPLIANCE RISKS')).toBeInTheDocument();
  });

  it('renders analytics charts section', () => {
    render(
      <BrowserRouter>
        <Dashboard />
      </BrowserRouter>
    );

    expect(screen.getByTestId('bar-chart')).toBeInTheDocument();
    expect(screen.getByTestId('area-chart')).toBeInTheDocument();
  });
});
