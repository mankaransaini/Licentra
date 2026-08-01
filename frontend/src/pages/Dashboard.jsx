import React, { useEffect, useState } from 'react';
import anime from 'animejs';
import { Link, useNavigate } from 'react-router-dom';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip as RechartsTooltip, ResponsiveContainer, AreaChart, Area } from 'recharts';
import api from '../services/api';

const Dashboard = () => {
  const navigate = useNavigate();
  const [counts, setCounts] = useState({ software: null, licenses: null, assignments: null, auditLogs: null });
  const [deptChartData, setDeptChartData] = useState([]);
  const [usageChartData, setUsageChartData] = useState([]);
  const [expiredCount, setExpiredCount] = useState(0);
  const [expiringSoonCount, setExpiringSoonCount] = useState(0);

  useEffect(() => {
    anime({
      targets: '.animate-up',
      translateY: [20, 0],
      opacity: [0, 1],
      delay: anime.stagger(100),
      duration: 800,
      easing: 'easeOutQuart'
    });

    const loadLiveStats = async () => {
      try {
        const [swRes, licRes, asgRes, auditRes, empRes, deptRes] = await Promise.allSettled([
          api.get('/software'),
          api.get('/license'),
          api.get('/licenseassignment'),
          api.get('/auditlog'),
          api.get('/employees'),
          api.get('/departments')
        ]);

        const getArray = (res) => (res.status === 'fulfilled' && Array.isArray(res.value.data) ? res.value.data : []);

        const swList = getArray(swRes);
        const licList = getArray(licRes);
        const asgList = getArray(asgRes);
        const auditList = getArray(auditRes);
        const empList = getArray(empRes);
        const deptList = getArray(deptRes);

        setCounts({
          software: swList.length,
          licenses: licList.length,
          assignments: asgList.length,
          auditLogs: auditList.length
        });

        // 1. Calculate Licenses by Department
        const deptMap = {};
        deptList.forEach(d => {
          deptMap[d.departmentName] = 0;
        });

        asgList.forEach(asg => {
          const emp = empList.find(e => e.employeeId === asg.employeeId || e.fullName === asg.employeeName);
          const deptName = emp?.departmentName || (emp?.department ? emp.department.departmentName : null);
          if (deptName) {
            deptMap[deptName] = (deptMap[deptName] || 0) + 1;
          } else if (asg.employeeName) {
            deptMap['General'] = (deptMap['General'] || 0) + 1;
          }
        });

        let formattedDept = Object.keys(deptMap).map(dName => ({
          name: dName,
          allocated: deptMap[dName]
        }));
        
        // Filter out empty clutter departments or take active ones for clean alignment
        const activeDepts = formattedDept.filter(d => d.allocated > 0);
        if (activeDepts.length > 0) {
          formattedDept = activeDepts;
        } else {
          formattedDept = formattedDept.slice(0, 5);
        }

        setDeptChartData(formattedDept.length > 0 ? formattedDept : [
          { name: 'General', allocated: asgList.length }
        ]);

        // 2. Calculate Active vs Total Licenses (6 Months Trend)
        const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        const now = new Date();
        const last6Months = [];
        for (let i = 5; i >= 0; i--) {
          const d = new Date(now.getFullYear(), now.getMonth() - i, 1);
          last6Months.push({
            monthKey: `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}`,
            monthLabel: monthNames[d.getMonth()],
            total: 0,
            active: 0
          });
        }

        last6Months.forEach(m => {
          const mEnd = new Date(m.monthKey + '-31T23:59:59');
          m.total = licList.filter(l => !l.createdAt || new Date(l.createdAt) <= mEnd).length;
          m.active = asgList.filter(a => !a.assignedDate || new Date(a.assignedDate) <= mEnd).length;
        });

        setUsageChartData(last6Months.map(m => ({
          month: m.monthLabel,
          active: m.active,
          total: m.total
        })));

        // 3. Calculate Expiry Alerts
        const today = new Date();
        const thirtyDays = new Date();
        thirtyDays.setDate(today.getDate() + 30);

        let expired = 0;
        let expiringSoon = 0;

        licList.forEach(l => {
          if (l.expiryDate) {
            const exp = new Date(l.expiryDate);
            if (exp < today) {
              expired++;
            } else if (exp <= thirtyDays) {
              expiringSoon++;
            }
          }
        });

        setExpiredCount(expired);
        setExpiringSoonCount(expiringSoon);

      } catch (e) {
        console.error("Could not fetch dashboard live stats", e);
      }
    };

    loadLiveStats();
  }, []);

  const totalLic = counts.licenses ?? 0;
  const totalAsg = counts.assignments ?? 0;
  const seatPercent = totalLic > 0 ? Math.round((totalAsg / totalLic) * 100) : 0;

  const topStats = [
    { title: 'SOFTWARE CATALOG', value: counts.software !== null ? String(counts.software) : '0', subtitle: 'Applications', icon: '💿', color: '#eef2ff', iconColor: '#6366f1', path: '/software', animClass: 'icon-spin' },
    { title: 'TOTAL LICENSES', value: counts.licenses !== null ? String(counts.licenses) : '0', subtitle: 'Registered Keys', icon: '🔑', color: '#f0fdf4', iconColor: '#16a34a', path: '/licenses', animClass: 'icon-wiggle' },
    { title: 'SEAT ALLOCATION', value: `${seatPercent}%`, subtitle: `(${totalAsg}/${totalLic})`, icon: '📁', color: '#fff7ed', iconColor: '#ea580c', path: '/assignments', animClass: 'icon-bounce' },
    { title: 'COMPLIANCE RISKS', value: String(expiredCount), subtitle: 'Alert Flags', icon: '🛡️', color: expiredCount > 0 ? '#fef2f2' : '#f0fdf4', iconColor: expiredCount > 0 ? '#dc2626' : '#16a34a', path: '/auditlogs', animClass: 'icon-pulse' }
  ];

  const trendingModules = [
    { name: 'Licenses', icon: '🔑', iconBg: '#f0fdf4', iconColor: '#16a34a', path: '/licenses', animClass: 'icon-wiggle' },
    { name: 'Assignments', icon: '📝', iconBg: '#fff7ed', iconColor: '#ea580c', path: '/assignments', animClass: 'icon-write' },
    { name: 'Employees', icon: '👥', iconBg: '#f0fdfa', iconColor: '#0d9488', path: '/employees', animClass: 'icon-float' },
    { name: 'Software', icon: '💿', iconBg: '#eef2ff', iconColor: '#6366f1', path: '/software', animClass: 'icon-spin' },
    { name: 'Vendors', icon: '🏢', iconBg: '#faf5ff', iconColor: '#9333ea', path: '/vendors', animClass: 'icon-bounce' },
    { name: 'Audit Logs', icon: '⏱️', iconBg: '#eff6ff', iconColor: '#2563eb', path: '/auditlogs', animClass: 'icon-tick' }
  ];

  return (
    <div className="fade-in" style={{ paddingBottom: '3rem' }}>
      
      {/* Hero Banner */}
      <div className="animate-up" style={{ 
        background: 'var(--navbar-bg)', 
        color: 'white', 
        borderRadius: 'var(--border-radius-lg)', 
        padding: '2rem 3rem',
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: '2rem',
        position: 'relative',
        overflow: 'hidden'
      }}>
        <div style={{ flex: 1, zIndex: 1, paddingRight: '2rem' }}>
          <div style={{ display: 'inline-flex', alignItems: 'center', gap: '0.5rem', background: 'rgba(255,255,255,0.1)', border: '1px solid rgba(255,255,255,0.2)', padding: '0.4rem 1rem', borderRadius: '30px', fontSize: '0.75rem', fontWeight: 'bold', marginBottom: '1.5rem', letterSpacing: '1px', backdropFilter: 'blur(10px)', boxShadow: '0 4px 15px rgba(0,0,0,0.1)' }}>
            <span style={{width: '8px', height: '8px', background: '#4ade80', borderRadius: '50%', boxShadow: '0 0 10px #4ade80'}}></span>
            COMPLIANCE STATUS: ACTIVE
          </div>
          <h1 style={{ fontSize: '3rem', fontWeight: '800', marginBottom: '1rem', color: 'white', lineHeight: '1.1', textShadow: '0 10px 30px rgba(0,0,0,0.3)' }}>
            Manage Software<br/>Licenses with <span style={{ color: 'var(--accent-primary)', textShadow: '0 0 20px rgba(249,115,22,0.4)' }}>Precision</span>
          </h1>
          <p style={{ color: 'rgba(255,255,255,0.8)', marginBottom: '2rem', maxWidth: '550px', lineHeight: '1.6', fontSize: '1.05rem' }}>
            Track allocations, oversee seat compliance, and control IT vendor spend. Got a new software application? Register a license key or coordinate employee assignments immediately.
          </p>
          <Link to="/licenses" className="btn btn-primary" style={{ padding: '0.85rem 1.75rem', fontSize: '1.05rem', borderRadius: '12px', textDecoration: 'none', display: 'inline-block' }}>
            Register New License &rarr;
          </Link>
        </div>
        
        <div style={{ 
          background: 'rgba(255,255,255,0.05)', 
          border: '1px solid rgba(255,255,255,0.1)', 
          borderRadius: '12px',
          padding: '1.5rem',
          width: '350px',
          zIndex: 1
        }}>
          <h4 style={{ fontSize: '0.75rem', color: 'rgba(255,255,255,0.6)', textTransform: 'uppercase', letterSpacing: '1px', marginBottom: '1rem' }}>Licentra Live Insights</h4>
          <div style={{ marginBottom: '1rem' }}>
            <div style={{ fontWeight: 'bold', fontSize: '0.875rem' }}>✓ Employee Assignments</div>
            <div style={{ fontSize: '0.75rem', color: 'rgba(255,255,255,0.6)', marginTop: '0.25rem' }}>
              {totalLic > 0 ? `${seatPercent}% of registered seat licenses are currently assigned in the database.` : 'Register licenses to track seat allocations.'}
            </div>
          </div>
          <div>
            <div style={{ fontWeight: 'bold', fontSize: '0.875rem' }}>⚠ Database Synchronized</div>
            <div style={{ fontSize: '0.75rem', color: 'rgba(255,255,255,0.6)', marginTop: '0.25rem' }}>
              {counts.licenses !== null ? `${counts.licenses} Total Licenses & ${counts.assignments} Active Assignments.` : 'Fetching metrics...'}
            </div>
          </div>
        </div>
      </div>

      {/* Top Stats */}
      <div className="animate-up" style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '1.5rem', marginBottom: '3rem' }}>
        {topStats.map((stat, i) => (
          <Link to={stat.path} key={i} className="card" style={{ padding: '1.5rem', display: 'flex', alignItems: 'center', gap: '1rem', textDecoration: 'none', color: 'inherit' }}>
            <div style={{ background: stat.color, color: stat.iconColor, width: '50px', height: '50px', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '1.5rem' }}>
              <span className={`animated-icon ${stat.animClass}`}>{stat.icon}</span>
            </div>
            <div>
              <div style={{ fontSize: '0.75rem', fontWeight: 'bold', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.5px' }}>{stat.title}</div>
              <div style={{ display: 'flex', alignItems: 'baseline', gap: '0.25rem' }}>
                <span style={{ fontSize: '2rem', fontWeight: 'bold' }}>{stat.value}</span>
                <span style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', fontWeight: '500' }}>{stat.subtitle}</span>
              </div>
            </div>
          </Link>
        ))}
      </div>

      {/* Analytics Charts */}
      <div className="animate-up" style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', marginBottom: '3rem' }}>
        
        {/* Department Allocation Chart */}
        <div className="card" style={{ padding: '1.5rem', display: 'flex', flexDirection: 'column' }}>
          <h3 style={{ fontSize: '0.875rem', textTransform: 'uppercase', color: 'var(--text-secondary)', letterSpacing: '1px', marginBottom: '1.5rem' }}>Licenses by Department</h3>
          <div style={{ width: '100%', height: '280px', minHeight: '280px' }}>
            <ResponsiveContainer width="99%" height={280}>
              <BarChart data={deptChartData} margin={{ top: 10, right: 20, left: -10, bottom: 25 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--border-color)" vertical={false} />
                <XAxis 
                  dataKey="name" 
                  tick={{ fill: 'var(--text-secondary)', fontSize: 11 }} 
                  axisLine={false} 
                  tickLine={false}
                  interval={0}
                  angle={-15}
                  textAnchor="end"
                  height={35}
                />
                <YAxis allowDecimals={false} tick={{fill: 'var(--text-secondary)', fontSize: 12}} axisLine={false} tickLine={false} />
                <RechartsTooltip 
                  contentStyle={{ backgroundColor: 'var(--bg-surface)', borderColor: 'var(--border-color)', borderRadius: '8px', boxShadow: 'var(--shadow-md)' }} 
                  itemStyle={{ color: 'var(--text-primary)' }}
                  cursor={{fill: 'rgba(249,115,22,0.1)'}} 
                />
                <Bar dataKey="allocated" name="Allocated Licenses" fill="url(#colorAllocated)" radius={[4, 4, 0, 0]} barSize={28} />
                <defs>
                  <linearGradient id="colorAllocated" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#f97316" stopOpacity={1}/>
                    <stop offset="95%" stopColor="#fb923c" stopOpacity={0.8}/>
                  </linearGradient>
                </defs>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Usage Trend Chart */}
        <div className="card" style={{ padding: '1.5rem', display: 'flex', flexDirection: 'column' }}>
          <h3 style={{ fontSize: '0.875rem', textTransform: 'uppercase', color: 'var(--text-secondary)', letterSpacing: '1px', marginBottom: '1.5rem' }}>Active vs Total Licenses (6 Months)</h3>
          <div style={{ width: '100%', height: '280px', minHeight: '280px' }}>
            <ResponsiveContainer width="99%" height={280}>
              <AreaChart data={usageChartData} margin={{ top: 10, right: 20, left: -10, bottom: 10 }}>
                <defs>
                  <linearGradient id="colorActive" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#10b981" stopOpacity={0.3}/>
                    <stop offset="95%" stopColor="#10b981" stopOpacity={0}/>
                  </linearGradient>
                  <linearGradient id="colorTotal" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#6366f1" stopOpacity={0.1}/>
                    <stop offset="95%" stopColor="#6366f1" stopOpacity={0}/>
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--border-color)" vertical={false} />
                <XAxis dataKey="month" tick={{fill: 'var(--text-secondary)', fontSize: 12}} axisLine={false} tickLine={false} />
                <YAxis allowDecimals={false} tick={{fill: 'var(--text-secondary)', fontSize: 12}} axisLine={false} tickLine={false} />
                <RechartsTooltip 
                  contentStyle={{ backgroundColor: 'var(--bg-surface)', borderColor: 'var(--border-color)', borderRadius: '8px', boxShadow: 'var(--shadow-md)' }} 
                  itemStyle={{ color: 'var(--text-primary)' }}
                />
                <Area type="monotone" dataKey="total" name="Total Licenses" stroke="#6366f1" strokeWidth={2} fillOpacity={1} fill="url(#colorTotal)" />
                <Area type="monotone" dataKey="active" name="Active Assignments" stroke="#10b981" strokeWidth={3} fillOpacity={1} fill="url(#colorActive)" />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Trending Modules */}
      <div className="animate-up" style={{ marginBottom: '3rem' }}>
        <h3 style={{ fontSize: '0.875rem', textTransform: 'uppercase', color: 'var(--text-secondary)', letterSpacing: '1px', marginBottom: '1rem' }}>Trending Modules</h3>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(6, 1fr)', gap: '1rem' }}>
          {trendingModules.map((module, i) => (
            <Link to={module.path} key={i} className="card" style={{ padding: '1.5rem 1rem', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: '1rem', cursor: 'pointer', transition: 'transform 0.2s', textDecoration: 'none', color: 'inherit' }}>
              <div style={{ background: module.iconBg, color: module.iconColor, width: '48px', height: '48px', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '1.25rem' }}>
                <span className={`animated-icon ${module.animClass}`}>{module.icon}</span>
              </div>
              <div style={{ fontWeight: '600', fontSize: '0.875rem' }}>{module.name}</div>
            </Link>
          ))}
        </div>
      </div>

      {/* Bottom Section */}
      <div className="animate-up" style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '1.5rem' }}>
        
        {/* Alerts */}
        <div className="card" style={{ overflow: 'hidden' }}>
          <div style={{ background: 'var(--accent-primary)', color: 'white', padding: '0.75rem 1.5rem', fontWeight: 'bold', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            ⚠ Critical System Alerts
          </div>
          <div style={{ padding: '2rem', display: 'flex', gap: '1.5rem' }}>
            <button 
              onClick={() => navigate('/licenses?filter=expired')} 
              style={{ flex: 1, border: '1px solid #fecaca', background: '#fef2f2', borderRadius: '8px', padding: '1.5rem', display: 'flex', alignItems: 'center', gap: '1rem', cursor: 'pointer', transition: 'all 0.2s', outline: 'none' }}
              onMouseEnter={(e) => e.currentTarget.style.background = '#fee2e2'}
              onMouseLeave={(e) => e.currentTarget.style.background = '#fef2f2'}
            >
              <div style={{ background: 'rgba(220, 38, 38, 0.1)', color: '#dc2626', width: '40px', height: '40px', borderRadius: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>🛡️</div>
              <div>
                <div style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#dc2626' }}>{expiredCount}</div>
                <div style={{ fontSize: '0.75rem', fontWeight: 'bold', color: '#dc2626', textTransform: 'uppercase' }}>Expired Licenses</div>
              </div>
            </button>
            
            <button 
              onClick={() => navigate('/licenses?filter=expiringsoon')} 
              style={{ flex: 1, border: '1px solid #fde68a', background: '#fffbeb', borderRadius: '8px', padding: '1.5rem', display: 'flex', alignItems: 'center', gap: '1rem', cursor: 'pointer', transition: 'all 0.2s', outline: 'none' }}
              onMouseEnter={(e) => e.currentTarget.style.background = '#fef3c7'}
              onMouseLeave={(e) => e.currentTarget.style.background = '#fffbeb'}
            >
              <div style={{ background: 'rgba(217, 119, 6, 0.1)', color: '#d97706', width: '40px', height: '40px', borderRadius: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>⚠</div>
              <div>
                <div style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#d97706' }}>{expiringSoonCount}</div>
                <div style={{ fontSize: '0.75rem', fontWeight: 'bold', color: '#d97706', textTransform: 'uppercase' }}>Expiring Soon</div>
              </div>
            </button>
          </div>
        </div>

        {/* Health */}
        <div className="card" style={{ overflow: 'hidden', display: 'flex', flexDirection: 'column' }}>
          <div style={{ background: 'var(--navbar-bg)', color: 'white', padding: '0.75rem 1.5rem', fontWeight: 'bold', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            📈 Key Metrics & Health
          </div>
          <div style={{ padding: '2rem', flex: 1, display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
            <div style={{ marginBottom: '2rem' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem', fontSize: '0.875rem', fontWeight: '500' }}>
                <span>Overall System Health</span>
                <span style={{ color: 'var(--accent-primary)' }}>100%</span>
              </div>
              <div style={{ height: '8px', background: '#f3f4f6', borderRadius: '4px', overflow: 'hidden' }}>
                <div style={{ height: '100%', width: '100%', background: 'linear-gradient(90deg, #ea580c, #f97316)', borderRadius: '4px' }}></div>
              </div>
            </div>
            
            <div style={{ marginTop: 'auto', display: 'flex', justifyContent: 'flex-end' }}>
              <Link to="/assignments" className="btn btn-primary" style={{ padding: '0.5rem 1.5rem', fontWeight: 'bold', textDecoration: 'none' }}>ALLOCATE</Link>
            </div>
          </div>
        </div>

      </div>

    </div>
  );
};

export default Dashboard;
