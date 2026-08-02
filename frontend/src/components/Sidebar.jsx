import React, { useContext } from 'react';
import { NavLink } from 'react-router-dom';
import { AuthContext } from '../contexts/AuthContext';

const Sidebar = ({ isCollapsed }) => {
  const { user } = useContext(AuthContext);
  const userRole = user?.role?.toLowerCase() || '';
  const isAdmin = userRole.includes('admin');
  const isManager = userRole.includes('manager');

  const navItems = [
    { name: 'Dashboard', path: '/', icon: '📈' },
    { name: 'Assigned', path: '/assigned', icon: '📥' },
    { name: 'Employees', path: '/employees', icon: '👥' },
    { name: 'Software', path: '/software', icon: '💿' },
    { name: 'Departments', path: '/departments', icon: '🏛️' },
  ];

  if (isAdmin || isManager) {
    navItems.splice(1, 0, { name: 'Licenses', path: '/licenses', icon: '🔑' });
    navItems.splice(2, 0, { name: 'Assignments', path: '/assignments', icon: '📝' });
  }

  if (isAdmin) {
    navItems.push({ name: 'Vendors', path: '/vendors', icon: '🏢' });
    navItems.push({ name: 'Roles', path: '/roles', icon: '🛡️' });
    navItems.push({ name: 'Users', path: '/users', icon: '👤' });
    navItems.push({ name: 'Audit Logs', path: '/auditlogs', icon: '⏱️' });
  }

  return (
    <div className={`sidebar ${isCollapsed ? 'collapsed' : ''}`}>
      <div style={{ padding: '1rem', flex: 1, overflowY: 'auto' }}>
        <nav style={{ display: 'flex', flexDirection: 'column', gap: '0.25rem' }}>
          {navItems.map((item) => (
            <NavLink
              key={item.name}
              to={item.path}
              className={({ isActive }) => 
                `nav-link ${isActive ? 'active' : ''}`
              }
              title={isCollapsed ? item.name : ''}
            >
              <span style={{ fontSize: '1.2rem', width: '24px', textAlign: 'center', flexShrink: 0 }}>{item.icon}</span>
              <span className="nav-link-text">{item.name}</span>
            </NavLink>
          ))}
        </nav>
      </div>
    </div>
  );
};

export default Sidebar;
