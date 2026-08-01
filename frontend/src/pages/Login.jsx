import React, { useState, useContext, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../contexts/AuthContext';
import anime from 'animejs';

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const { login, error } = useContext(AuthContext);
  const navigate = useNavigate();
  const formRef = useRef(null);

  useEffect(() => {
    anime({
      targets: formRef.current,
      translateY: [40, 0],
      opacity: [0, 1],
      easing: 'easeOutQuart',
      duration: 1200,
    });
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Button click animation
    anime({
      targets: '.login-btn',
      scale: [1, 0.95, 1],
      duration: 300,
      easing: 'easeInOutQuad'
    });

    const success = await login(username, password);
    if (success) {
      anime({
        targets: formRef.current,
        translateY: [0, -30],
        opacity: [1, 0],
        easing: 'easeInExpo',
        duration: 500,
        complete: () => navigate('/')
      });
    } else {
      anime({
        targets: formRef.current,
        translateX: [
          { value: -10, duration: 50 },
          { value: 10, duration: 50 },
          { value: -10, duration: 50 },
          { value: 10, duration: 50 },
          { value: 0, duration: 50 }
        ],
        easing: 'easeInOutQuad'
      });
    }
  };

  return (
    <div style={{ 
      minHeight: '100vh', 
      display: 'flex', 
      alignItems: 'center', 
      justifyContent: 'center',
      position: 'relative',
      overflow: 'hidden'
    }}>
      
      {/* Background Blobs for depth */}
      <div style={{ position: 'absolute', top: '-10%', left: '-5%', width: '500px', height: '500px', background: 'var(--accent-primary)', filter: 'blur(150px)', opacity: 0.15, borderRadius: '50%', zIndex: 0, animation: 'float 10s infinite alternate' }}></div>
      <div style={{ position: 'absolute', bottom: '-10%', right: '-5%', width: '600px', height: '600px', background: 'var(--navbar-bg)', filter: 'blur(150px)', opacity: 0.2, borderRadius: '50%', zIndex: 0, animation: 'float 12s infinite alternate-reverse' }}></div>

      <div ref={formRef} className="card" style={{ 
        width: '100%', 
        maxWidth: '440px', 
        padding: '3rem 2.5rem', 
        zIndex: 1, 
        background: 'rgba(255, 255, 255, 0.85)', 
        backdropFilter: 'blur(20px)',
        border: '1px solid rgba(255, 255, 255, 0.4)',
        boxShadow: '0 25px 50px -12px rgba(11, 28, 60, 0.25)'
      }}>
        
        <div style={{ textAlign: 'center', marginBottom: '2.5rem' }}>
          <div style={{ background: 'var(--accent-gradient)', width: '48px', height: '48px', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'white', margin: '0 auto 1rem auto', fontSize: '1.5rem', fontWeight: 'bold', boxShadow: '0 10px 20px rgba(249, 115, 22, 0.3)' }}>L</div>
          <h2 style={{ fontSize: '1.8rem', color: 'var(--navbar-bg)', marginBottom: '0.5rem' }}>Welcome to Licentra</h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>Enter your credentials to access the hub.</p>
        </div>

        {error && (
          <div style={{ background: 'var(--danger-bg)', color: 'var(--danger-text)', padding: '0.75rem', borderRadius: 'var(--border-radius-sm)', marginBottom: '1.5rem', fontSize: '0.85rem', textAlign: 'center', border: '1px solid rgba(220, 38, 38, 0.2)' }}>
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
          <div>
            <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: '600', color: 'var(--text-secondary)', marginBottom: '0.5rem' }}>USERNAME</label>
            <input 
              type="text" 
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              style={{ width: '100%', padding: '0.85rem 1rem', borderRadius: '10px', border: '1px solid var(--border-color)', background: 'var(--bg-main)', fontSize: '0.95rem', transition: 'all 0.3s', fontFamily: 'inherit' }}
              placeholder="admin"
              onFocus={(e) => { e.target.style.borderColor = 'var(--accent-primary)'; e.target.style.boxShadow = '0 0 0 3px rgba(249, 115, 22, 0.1)'; }}
              onBlur={(e) => { e.target.style.borderColor = 'var(--border-color)'; e.target.style.boxShadow = 'none'; }}
            />
          </div>
          
          <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
              <label style={{ fontSize: '0.85rem', fontWeight: '600', color: 'var(--text-secondary)' }}>PASSWORD</label>
              <span style={{ fontSize: '0.75rem', color: 'var(--accent-primary)', cursor: 'pointer', fontWeight: '500' }}>Forgot?</span>
            </div>
            <input 
              type="password" 
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              style={{ width: '100%', padding: '0.85rem 1rem', borderRadius: '10px', border: '1px solid var(--border-color)', background: 'var(--bg-main)', fontSize: '0.95rem', transition: 'all 0.3s', fontFamily: 'inherit' }}
              placeholder="••••••••"
              onFocus={(e) => { e.target.style.borderColor = 'var(--accent-primary)'; e.target.style.boxShadow = '0 0 0 3px rgba(249, 115, 22, 0.1)'; }}
              onBlur={(e) => { e.target.style.borderColor = 'var(--border-color)'; e.target.style.boxShadow = 'none'; }}
            />
          </div>
          
          <button type="submit" className="btn btn-primary login-btn" style={{ padding: '1rem', marginTop: '1rem', fontSize: '1rem', borderRadius: '10px', fontWeight: '700', letterSpacing: '0.05em' }}>
            SIGN IN
          </button>
        </form>

      </div>
    </div>
  );
};

export default Login;
