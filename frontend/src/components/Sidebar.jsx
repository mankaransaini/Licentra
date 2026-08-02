import React, { useContext } from 'react';
import { NavLink } from 'react-router-dom';
import { AuthContext } from '../contexts/AuthContext';

const Sidebar = () => {
  const { user } = useContext(AuthContext);
  const isAdmin = user?.role?.toLowerCase().includes('admin') || user?.username?.toLowerCase() === 'admin';

  const navItems = [
    { name: 'Dashboard', path: '/', icon: '📈' },
    { name: 'Licenses', path: '/licenses', icon: '🔑' },
    { name: 'Assignments', path: '/assignments', icon: '📝' },
    { name: 'Employees', path: '/employees', icon: '👥' },
    { name: 'Software', path: '/software', icon: '💿' },
    { name: 'Vendors', path: '/vendors', icon: '🏢' },
    { name: 'Departments', path: '/departments', icon: '🏛️' },
    { name: 'Roles', path: '/roles', icon: '🛡️' },
    { name: 'Users', path: '/users', icon: '👤' },
  ];

  if (isAdmin) {
    navItems.push({ name: 'Audit Logs', path: '/auditlogs', icon: '⏱️' });
  }

  return (
    <div className="sidebar">
      <div style={{ padding: '1rem', flex: 1, overflowY: 'auto' }}>
        <nav style={{ display: 'flex', flexDirection: 'column', gap: '0.25rem' }}>
          {navItems.map((item) => (
            <NavLink
              key={item.name}
              to={item.path}
              className={({ isActive }) => 
                `nav-link ${isActive ? 'active' : ''}`
              }
            >
              <span style={{ fontSize: '1.2rem', width: '24px', textAlign: 'center' }}>{item.icon}</span>
              {item.name}
            </NavLink>
          ))}
        </nav>
      </div>
    </div>
  );
};

export default Sidebar;
