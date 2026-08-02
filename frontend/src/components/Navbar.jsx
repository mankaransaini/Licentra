import React, { useContext, useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../contexts/AuthContext';

const SEARCH_DATA = [
  // Modules
  { id: '0.1', title: 'Dashboard', category: 'Module', path: '/dashboard', icon: '�' },
  { id: '0.2', title: 'Licenses', category: 'Module', path: '/licenses', icon: '🔑' },
  { id: '0.3', title: 'Assignments', category: 'Module', path: '/assignments', icon: '📋' },
  { id: '0.4', title: 'Employees', category: 'Module', path: '/employees', icon: '👥' },
  { id: '0.5', title: 'Software', category: 'Module', path: '/software', icon: '💿' },
  { id: '0.6', title: 'Vendors', category: 'Module', path: '/vendors', icon: '🏢' },
  { id: '0.7', title: 'Departments', category: 'Module', path: '/departments', icon: '🏛️' },
  { id: '0.8', title: 'Roles', category: 'Module', path: '/roles', icon: '🔐' },
  { id: '0.9', title: 'Users', category: 'Module', path: '/users', icon: '👤' },
  
  // Software
  { id: '1', title: 'Microsoft 365 Enterprise', category: 'Software', path: '/software', icon: '💿' },
  { id: '2', title: 'Adobe Creative Cloud', category: 'Software', path: '/software', icon: '💿' },
  { id: '3', title: 'Figma Professional', category: 'Software', path: '/software', icon: '💿' },
  { id: '4', title: 'Salesforce CRM', category: 'Software', path: '/software', icon: '💿' },
  { id: '5', title: 'Slack Business Plus', category: 'Software', path: '/software', icon: '💿' },
  
  // Employees
  { id: '6', title: 'Alex Johnson - Senior Lead', category: 'Employees', path: '/employees', icon: '👥' },
  { id: '7', title: 'Sarah Connor - Product Lead', category: 'Employees', path: '/employees', icon: '👥' },
  { id: '8', title: 'David Miller - DevOps', category: 'Employees', path: '/employees', icon: '👥' },
  
  // Licenses
  { id: '9', title: 'LIC-8849-MSFT (Expires 2026)', category: 'Licenses', path: '/licenses', icon: '🔑' },
  { id: '10', title: 'LIC-9921-ADBE (Active)', category: 'Licenses', path: '/licenses', icon: '🔑' },
  
  // Vendors
  { id: '11', title: 'Microsoft Corporation', category: 'Vendors', path: '/vendors', icon: '🏢' },
  { id: '12', title: 'Adobe Systems Inc', category: 'Vendors', path: '/vendors', icon: '🏢' },
  
  // Users
  { id: '13', title: 'admin (Super Administrator)', category: 'Users', path: '/users', icon: '👤' },
  { id: '14', title: 'johndoe (License Manager)', category: 'Users', path: '/users', icon: '👤' },
];

const Navbar = ({ isSidebarCollapsed, setIsSidebarCollapsed }) => {
  const { user, logout } = useContext(AuthContext);
  const navigate = useNavigate();
  const [fontSize, setFontSize] = useState('normal');
  const [isDarkMode, setIsDarkMode] = useState(false);
  const [animatingTheme, setAnimatingTheme] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [isSearchOpen, setIsSearchOpen] = useState(false);
  const searchRef = useRef(null);

  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const profileRef = useRef(null);

  useEffect(() => {
    const html = document.documentElement;
    if (fontSize === 'small') {
      html.style.fontSize = '12px';
    } else if (fontSize === 'large') {
      html.style.fontSize = '16px';
    } else {
      html.style.fontSize = '14px';
    }
  }, [fontSize]);

  useEffect(() => {
    if (isDarkMode) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
  }, [isDarkMode]);

  // Close search & profile dropdowns on click outside
  useEffect(() => {
    const handleClickOutside = (e) => {
      if (searchRef.current && !searchRef.current.contains(e.target)) {
        setIsSearchOpen(false);
      }
      if (profileRef.current && !profileRef.current.contains(e.target)) {
        setIsProfileOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const toggleTheme = () => {
    setAnimatingTheme(true);
    setIsDarkMode(!isDarkMode);
    setTimeout(() => setAnimatingTheme(false), 400);
  };

  const filteredResults = searchQuery.trim() === '' ? [] : SEARCH_DATA.filter(item => 
    item.title.toLowerCase().includes(searchQuery.toLowerCase()) || 
    item.category.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const handleSelectResult = (path) => {
    setIsSearchOpen(false);
    setSearchQuery('');
    navigate(path);
  };

  if (!user) return null;

  return (
    <div className="top-navbar">
      <div style={{ display: 'flex', alignItems: 'center', gap: '1.5rem' }}>
        <div style={{ fontWeight: 'bold', fontSize: '1.25rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
          <div style={{ background: 'var(--accent-gradient)', width: '30px', height: '30px', borderRadius: '4px', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'white' }}>L</div>
          <span>LICENTRA <span style={{fontSize: '0.6rem', color: 'var(--accent-primary)', verticalAlign: 'middle', marginLeft: '2px'}}>LICENSE HUB</span></span>
        </div>
        
        {/* Hamburger Toggle */}
        <button 
          onClick={() => setIsSidebarCollapsed(!isSidebarCollapsed)}
          style={{
            background: 'transparent',
            border: 'none',
            cursor: 'pointer',
            padding: '0.25rem',
            width: '32px',
            height: '32px',
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'center',
            alignItems: 'center',
            gap: '6px'
          }}
          title="Toggle Sidebar"
        >
          <span style={{
            width: '24px',
            height: '2px',
            background: 'var(--navbar-text)',
            borderRadius: '2px',
            transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
            transformOrigin: 'right center',
            transform: isSidebarCollapsed ? 'translateY(8px) rotate(-45deg) scaleX(0.5)' : 'translateY(0) rotate(0) scaleX(1)'
          }} />
          <span style={{
            width: '24px',
            height: '2px',
            background: 'var(--navbar-text)',
            borderRadius: '2px',
            transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
            opacity: 1
          }} />
          <span style={{
            width: '24px',
            height: '2px',
            background: 'var(--navbar-text)',
            borderRadius: '2px',
            transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
            transformOrigin: 'right center',
            transform: isSidebarCollapsed ? 'translateY(-8px) rotate(45deg) scaleX(0.5)' : 'translateY(0) rotate(0) scaleX(1)'
          }} />
        </button>
      </div>

      {/* Interactive Search Bar */}
      <div ref={searchRef} style={{ position: 'relative', width: '360px' }}>
        <input 
          type="text" 
          placeholder="Search Application or People Here..." 
          className="search-bar"
          value={searchQuery}
          onChange={(e) => {
            setSearchQuery(e.target.value);
            setIsSearchOpen(true);
          }}
          onFocus={() => setIsSearchOpen(true)}
          style={{ width: '100%' }}
        />

        {/* Search Results Dropdown */}
        {isSearchOpen && searchQuery.trim() !== '' && (
          <div style={{
            position: 'absolute',
            top: 'calc(100% + 8px)',
            left: 0,
            right: 0,
            background: 'var(--bg-surface)',
            border: '1px solid var(--border-color)',
            borderRadius: '12px',
            boxShadow: 'var(--shadow-lg)',
            zIndex: 1000,
            maxHeight: '320px',
            overflowY: 'auto',
            backdropFilter: 'blur(16px)',
            padding: '0.5rem 0'
          }}>
            {filteredResults.length > 0 ? (
              filteredResults.map((item) => (
                <div 
                  key={item.id}
                  onClick={() => handleSelectResult(item.path)}
                  style={{
                    padding: '0.65rem 1rem',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    cursor: 'pointer',
                    transition: 'background 0.2s',
                    fontSize: '0.875rem'
                  }}
                  onMouseEnter={(e) => e.currentTarget.style.background = 'rgba(249, 115, 22, 0.1)'}
                  onMouseLeave={(e) => e.currentTarget.style.background = 'transparent'}
                >
                  <div style={{ display: 'flex', alignItems: 'center', gap: '0.6rem' }}>
                    <span>{item.icon}</span>
                    <span style={{ fontWeight: '500', color: 'var(--text-primary)' }}>{item.title}</span>
                  </div>
                  <span style={{ fontSize: '0.7rem', padding: '0.2rem 0.5rem', background: 'rgba(255,255,255,0.08)', borderRadius: '4px', color: 'var(--text-secondary)', textTransform: 'uppercase', fontWeight: 'bold' }}>
                    {item.category}
                  </span>
                </div>
              ))
            ) : (
              <div style={{ padding: '1rem', textAlign: 'center', color: 'var(--text-secondary)', fontSize: '0.85rem' }}>
                No applications or employees found matching "{searchQuery}"
              </div>
            )}
          </div>
        )}
      </div>

      <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
        <div style={{ display: 'flex', gap: '2px', background: 'rgba(255,255,255,0.05)', padding: '4px', borderRadius: '8px', border: '1px solid rgba(255,255,255,0.1)' }}>
          <button 
            className={`btn-icon ${fontSize === 'small' ? 'active' : ''}`} 
            onClick={() => setFontSize('small')}
            style={{ border: 'none', background: fontSize === 'small' ? 'rgba(255,255,255,0.2)' : 'transparent' }}
          >A-</button>
          <button 
            className={`btn-icon ${fontSize === 'normal' ? 'active' : ''}`} 
            onClick={() => setFontSize('normal')}
            style={{ border: 'none', background: fontSize === 'normal' ? 'rgba(255,255,255,0.2)' : 'transparent' }}
          >A</button>
          <button 
            className={`btn-icon ${fontSize === 'large' ? 'active' : ''}`} 
            onClick={() => setFontSize('large')}
            style={{ border: 'none', background: fontSize === 'large' ? 'rgba(255,255,255,0.2)' : 'transparent' }}
          >A+</button>
        </div>
        
        {/* Dark mode toggle */}
        <button 
          className={`btn-icon ${animatingTheme ? 'theme-toggle-anim' : ''}`} 
          style={{border: 'none', background: 'transparent', fontSize: '1.2rem'}}
          onClick={toggleTheme}
        >
          {isDarkMode ? '☀️' : '🌙'}
        </button>
        
        {/* Interactive Profile & About Badge */}
        <div ref={profileRef} style={{ position: 'relative' }}>
          <div 
            onClick={() => setIsProfileOpen(!isProfileOpen)}
            style={{ 
              display: 'flex', 
              alignItems: 'center', 
              gap: '0.5rem', 
              cursor: 'pointer',
              padding: '0.25rem 0.5rem',
              borderRadius: '8px',
              transition: 'background 0.2s'
            }}
            onMouseEnter={(e) => e.currentTarget.style.background = 'rgba(255,255,255,0.1)'}
            onMouseLeave={(e) => e.currentTarget.style.background = 'transparent'}
          >
            <div style={{ textAlign: 'right', lineHeight: '1.2' }}>
              <div style={{ fontSize: '0.8rem', fontWeight: 'bold' }}>{user.username || 'User'}</div>
              <div style={{ fontSize: '0.6rem', color: 'var(--accent-primary)', fontWeight: '700', textTransform: 'uppercase' }}>{user.role || 'User'}</div>
            </div>
            <div style={{ background: 'var(--accent-gradient)', width: '36px', height: '36px', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', fontWeight: 'bold', fontSize: '0.8rem', color: 'white', boxShadow: '0 4px 10px rgba(249, 115, 22, 0.3)' }}>
              {user.username ? user.username.substring(0, 2).toUpperCase() : 'U'}
            </div>
          </div>

          {/* About / Profile Popover Menu */}
          {isProfileOpen && (
            <div style={{
              position: 'absolute',
              top: 'calc(100% + 12px)',
              right: 0,
              width: '280px',
              background: 'var(--bg-surface)',
              border: '1px solid var(--border-color)',
              borderRadius: '16px',
              boxShadow: 'var(--shadow-lg)',
              zIndex: 1000,
              backdropFilter: 'blur(16px)',
              overflow: 'hidden',
              animation: 'fadeIn 0.2s ease-out'
            }}>
              {/* Header */}
              <div style={{ padding: '1.25rem', borderBottom: '1px solid var(--border-color)', background: 'rgba(249, 115, 22, 0.05)' }}>
                <div style={{ fontSize: '0.95rem', fontWeight: 'bold', color: 'var(--text-primary)' }}>{user.role || 'User Role'}</div>
                <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginTop: '2px' }}>
                  {user.email || 'Missing Email (Please Restart Backend)'}
                </div>
                {user.employeeId && (
                  <div style={{ fontSize: '0.70rem', color: 'var(--text-secondary)', marginTop: '2px' }}>
                    Emp ID: {user.employeeId}
                  </div>
                )}
                <div style={{ display: 'inline-block', marginTop: '0.5rem', padding: '0.2rem 0.5rem', background: '#dcfce7', color: '#15803d', borderRadius: '20px', fontSize: '0.65rem', fontWeight: 'bold' }}>
                  ● Active Session
                </div>
              </div>

              {/* About Platform Stats */}
              <div style={{ padding: '1rem', borderBottom: '1px solid var(--border-color)', fontSize: '0.8rem' }}>
                <div style={{ fontSize: '0.7rem', fontWeight: 'bold', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.5px', marginBottom: '0.5rem' }}>About Licentra</div>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.4rem' }}>
                  <span style={{ color: 'var(--text-secondary)' }}>System Version</span>
                  <span style={{ fontWeight: '600' }}>v2.4.0 (Enterprise)</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.4rem' }}>
                  <span style={{ color: 'var(--text-secondary)' }}>Tech Stack</span>
                  <span style={{ fontWeight: '600' }}>React + .NET 8 API</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: 'var(--text-secondary)' }}>License Engine</span>
                  <span style={{ color: '#16a34a', fontWeight: '600' }}>Optimal</span>
                </div>
              </div>

              {/* Actions */}
              <div style={{ padding: '0.5rem' }}>
                <div 
                  onClick={() => { setIsProfileOpen(false); navigate('/auditlogs'); }}
                  style={{ padding: '0.6rem 0.75rem', borderRadius: '8px', cursor: 'pointer', fontSize: '0.85rem', display: 'flex', alignItems: 'center', gap: '0.5rem', transition: 'background 0.2s' }}
                  onMouseEnter={(e) => e.currentTarget.style.background = 'rgba(255,255,255,0.05)'}
                  onMouseLeave={(e) => e.currentTarget.style.background = 'transparent'}
                >
                  <span>🛡️</span> Security & Audit Log
                </div>
                <div 
                  onClick={logout}
                  style={{ padding: '0.6rem 0.75rem', borderRadius: '8px', cursor: 'pointer', fontSize: '0.85rem', display: 'flex', alignItems: 'center', gap: '0.5rem', color: '#ef4444', fontWeight: '600', transition: 'background 0.2s' }}
                  onMouseEnter={(e) => e.currentTarget.style.background = 'rgba(239, 68, 68, 0.1)'}
                  onMouseLeave={(e) => e.currentTarget.style.background = 'transparent'}
                >
                  <span>🚪</span> Sign Out
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Navbar;
