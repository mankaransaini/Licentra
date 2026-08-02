import React, { useState, useEffect, useRef, useContext } from 'react';
import { useLocation } from 'react-router-dom';
import anime from 'animejs';
import api from '../services/api';
import { AuthContext } from '../contexts/AuthContext';

// Custom Dropdown Component with right-aligned ID display and search
const CustomDropdown = ({ value, onChange, options, label, disabled = false, required = false, labelKey, valueKey }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const dropdownRef = useRef(null);
  const buttonRef = useRef(null);
  const searchInputRef = useRef(null);

  useEffect(() => {
    const handleClickOutside = (e) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
        setIsOpen(false);
        setSearchTerm('');
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  useEffect(() => {
    if (isOpen && searchInputRef.current) {
      searchInputRef.current.focus();
    }
  }, [isOpen]);

  const selectedOption = options.find(opt => opt[valueKey] == value);
  const displayLabel = selectedOption ? (selectedOption[labelKey] || selectedOption.name || selectedOption[valueKey]) : `-- Select ${label} --`;
  const displayId = selectedOption ? selectedOption[valueKey] : '';

  const filteredOptions = searchTerm.trim() === '' 
    ? options 
    : options.filter(opt => {
        const label = String(opt[labelKey] || opt.name || opt[valueKey]).toLowerCase();
        const id = String(opt[valueKey]).toLowerCase();
        const search = searchTerm.toLowerCase();
        return label.includes(search) || id.includes(search);
      });

  return (
    <div ref={dropdownRef} style={{ position: 'relative', width: '100%' }}>
      <button
        ref={buttonRef}
        type="button"
        onClick={() => !disabled && setIsOpen(!isOpen)}
        disabled={disabled}
        style={{
          width: '100%',
          padding: '0.65rem 0.85rem',
          borderRadius: '8px',
          border: '1px solid var(--border-color)',
          background: disabled ? 'var(--bg-tertiary)' : 'var(--bg-main)',
          color: 'var(--text-primary)',
          fontSize: '0.9rem',
          opacity: disabled ? 0.6 : 1,
          cursor: disabled ? 'not-allowed' : 'pointer',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          fontFamily: 'monospace',
          textAlign: 'left'
        }}
      >
        <span>{displayLabel}</span>
        <span style={{ fontSize: '0.75rem', color: 'var(--accent-primary)' }}>({displayId ? `ID: ${displayId}` : ''})</span>
      </button>

      {isOpen && !disabled && (
        <div style={{
          position: 'absolute',
          top: 'calc(100% + 4px)',
          left: 0,
          right: 0,
          width: '100%',
          background: 'var(--bg-surface)',
          border: '1px solid var(--border-color)',
          borderRadius: '8px',
          zIndex: 1000,
          maxHeight: '350px',
          boxShadow: 'var(--shadow-lg)',
          fontFamily: 'monospace',
          display: 'flex',
          flexDirection: 'column',
          overflow: 'hidden'
        }}>
          {/* Search Input */}
          <input
            ref={searchInputRef}
            type="text"
            placeholder={`Search by name or ID...`}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            style={{
              padding: '0.65rem 0.85rem',
              borderBottom: '1px solid var(--border-color)',
              border: 'none',
              borderRadius: '8px 8px 0 0',
              background: 'var(--bg-main)',
              color: 'var(--text-primary)',
              fontSize: '0.9rem',
              outline: 'none',
              flexShrink: 0
            }}
            onClick={(e) => e.stopPropagation()}
          />

          {/* Options List */}
          <div style={{ overflowY: 'auto', flex: 1, minHeight: '0' }}>
            <div
              onClick={() => {
                onChange('');
                setIsOpen(false);
                setSearchTerm('');
              }}
              style={{
                padding: '0.65rem 0.85rem',
                cursor: 'pointer',
                background: value === '' ? 'rgba(249, 115, 22, 0.2)' : 'transparent',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                borderBottom: '1px solid var(--border-color)',
                color: 'var(--text-primary)',
                fontSize: '0.9rem'
              }}
              onMouseEnter={(e) => e.currentTarget.style.background = 'rgba(249, 115, 22, 0.1)'}
              onMouseLeave={(e) => e.currentTarget.style.background = value === '' ? 'rgba(249, 115, 22, 0.2)' : 'transparent'}
            >
              <span>-- Select {label} --</span>
            </div>
            {filteredOptions.length > 0 ? (
              filteredOptions.map((opt) => (
                <div
                  key={opt[valueKey]}
                  onClick={() => {
                    onChange(opt[valueKey]);
                    setIsOpen(false);
                    setSearchTerm('');
                  }}
                  style={{
                    padding: '0.65rem 0.85rem',
                    cursor: 'pointer',
                    background: value == opt[valueKey] ? 'rgba(249, 115, 22, 0.2)' : 'transparent',
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                    color: 'var(--text-primary)',
                    fontSize: '0.9rem',
                    borderBottom: '1px solid rgba(255,255,255,0.05)',
                    whiteSpace: 'nowrap'
                  }}
                  onMouseEnter={(e) => e.currentTarget.style.background = 'rgba(249, 115, 22, 0.1)'}
                  onMouseLeave={(e) => e.currentTarget.style.background = value == opt[valueKey] ? 'rgba(249, 115, 22, 0.2)' : 'transparent'}
                >
                  <span>{opt[labelKey] || opt.name || opt[valueKey]}</span>
                  <span style={{ fontSize: '0.75rem', color: 'var(--accent-primary)', marginLeft: '1rem', flexShrink: 0 }}>ID: {opt[valueKey]}</span>
                </div>
              ))
            ) : (
              <div style={{
                padding: '1rem 0.85rem',
                textAlign: 'center',
                color: 'var(--text-secondary)',
                fontSize: '0.85rem'
              }}>
                No results found
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

const CrudPage = ({ title, endpoint: endpointProp, columns: columnsProp, config }) => {
  const { user } = useContext(AuthContext);
  const isAdmin = user?.role?.toLowerCase().includes('admin') || user?.username?.toLowerCase() === 'admin';
  
  const location = useLocation();
  const endpoint = config?.endpoint || endpointProp;
  const predefinedColumns = config?.columns || columnsProp || [];
  const idKey = config?.idKey || 'id';
  const formFields = config?.formFields || [];
  const readOnly = config?.readOnly || false;

  const [data, setData] = useState([]);
  const [displayColumns, setDisplayColumns] = useState(predefinedColumns);
  const [loading, setLoading] = useState(true);
  const [errorMsg, setErrorMsg] = useState('');
  
  // Modal state
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState(null);
  const [formData, setFormData] = useState({});
  const [saving, setSaving] = useState(false);
  const [formError, setFormError] = useState('');

  const tableRef = useRef(null);

  useEffect(() => {
    fetchData();
  }, [endpoint]);

  const generateColumnsFromData = (items) => {
    if (items.length === 0) return predefinedColumns;
    
    // Get all keys from the first item
    const allKeys = Object.keys(items[0]);
    
    // Generate columns from all keys
    let generatedColumns = allKeys.map(key => ({
      key,
      label: key
        .replace(/([A-Z])/g, ' $1') // Add space before capital letters
        .replace(/^./, str => str.toUpperCase()) // Capitalize first letter
        .trim()
    }));
    
    if (!isAdmin) {
      generatedColumns = generatedColumns.filter(col => {
        const lowerKey = col.key.toLowerCase();
        return !lowerKey.includes('assignedby') && !lowerKey.includes('createdat');
      });
    }

    return generatedColumns;
  };

  const fetchData = async () => {
    setLoading(true);
    setErrorMsg('');
    try {
      const response = await api.get(endpoint);
      const items = Array.isArray(response.data) ? response.data : (response.data ? [response.data] : []);
      setData(items);
      
      // Generate columns from all database fields
      const cols = generateColumnsFromData(items);
      setDisplayColumns(cols);
      
      setLoading(false);

      // Stagger animation for rows
      setTimeout(() => {
        anime({
          targets: '.table-row',
          translateY: [20, 0],
          opacity: [0, 1],
          delay: anime.stagger(60),
          duration: 400,
          easing: 'easeOutExpo'
        });
      }, 50);
    } catch (error) {
      console.error(`Error fetching data for ${endpoint}:`, error);
      setErrorMsg('Failed to load data from backend server.');
      setData([]);
      setLoading(false);
    }
  };

  const getItemId = (item) => {
    if (!item) return null;
    if (item[idKey] !== undefined) return item[idKey];
    if (item.id !== undefined) return item.id;
    // Fallback search for any key ending with Id
    const altKey = Object.keys(item).find(k => k.toLowerCase().endsWith('id'));
    return altKey ? item[altKey] : null;
  };

  const handleDelete = async (item) => {
    const id = getItemId(item);
    if (!id) return;
    if (!window.confirm('Are you sure you want to delete this record?')) return;

    try {
      await api.delete(`${endpoint}/${id}`);
      anime({
        targets: `#row-${id}`,
        translateX: [0, 50],
        opacity: [1, 0],
        duration: 300,
        easing: 'easeInQuad',
        complete: () => {
          setData(prev => prev.filter(i => getItemId(i) !== id));
        }
      });
    } catch (err) {
      alert(err.response?.data?.message || err.message || 'Failed to delete record.');
    }
  };

  const [optionsMap, setOptionsMap] = useState({});

  const loadSelectOptions = async () => {
    const selectFields = formFields.filter(f => f.type === 'select' && f.endpoint);
    for (const field of selectFields) {
      try {
        const res = await api.get(field.endpoint);
        const list = Array.isArray(res.data) ? res.data : (res.data ? [res.data] : []);
        setOptionsMap(prev => ({ ...prev, [field.name]: list }));
      } catch (e) {
        console.error(`Failed to load options for field ${field.name}:`, e);
      }
    }
  };

  const openAddModal = () => {
    setEditingItem(null);
    const initialForm = {};
    formFields.forEach(f => {
      if (typeof f.defaultValue === 'function') {
        initialForm[f.name] = f.defaultValue();
      } else if (f.defaultValue !== undefined) {
        initialForm[f.name] = f.defaultValue;
      } else {
        initialForm[f.name] = '';
      }
    });
    setFormData(initialForm);
    setFormError('');
    setIsModalOpen(true);
    loadSelectOptions();
  };

  const openEditModal = (item) => {
    setEditingItem(item);
    const initialForm = {};
    formFields.forEach(f => {
      initialForm[f.name] = item[f.name] !== undefined && item[f.name] !== null ? item[f.name] : '';
    });
    setFormData(initialForm);
    setFormError('');
    setIsModalOpen(true);
    loadSelectOptions();
  };

  const handleInputChange = (fieldName, value) => {
    setFormData(prev => {
      const updated = { ...prev, [fieldName]: value };
      // Clear any dependent fields when parent field changes
      formFields.forEach(field => {
        if (field.dependsOn === fieldName && field.filterKey) {
          updated[field.name] = '';
        }
      });
      return updated;
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setFormError('');

    // Format payload before posting
    const payload = { ...formData };
    formFields.forEach(f => {
      if (typeof f.hideIf === 'function' && f.hideIf(payload)) {
        payload[f.name] = null;
      } else if (payload[f.name] !== '' && payload[f.name] !== null && payload[f.name] !== undefined) {
        if (f.type === 'number' || (f.type === 'select' && f.valueKey)) {
          const num = Number(payload[f.name]);
          if (!isNaN(num)) payload[f.name] = num;
        }
      }
    });

    // Helper to safely convert status strings to tinyint byte numbers for DB
    const parseStatusByte = (val, defaultVal = 1) => {
      if (val === undefined || val === null || val === '') return defaultVal;
      if (typeof val === 'number') return val;
      const num = Number(val);
      if (!isNaN(num)) return num;
      const lower = String(val).toLowerCase();
      if (lower.includes('active') || lower.includes('valid') || lower.includes('assigned')) return 1;
      if (lower.includes('expired') || lower.includes('returned') || lower.includes('inactive')) return 2;
      if (lower.includes('pending') || lower.includes('suspended')) return 3;
      return defaultVal;
    };

    // Provide default required fields for backend DTOs if omitted
    const todayStr = new Date().toISOString().split('T')[0];
    if (payload.purchaseDate === undefined || payload.purchaseDate === '') payload.purchaseDate = todayStr;
    payload.licenseStatus = parseStatusByte(payload.licenseStatus, 1);
    if (payload.joiningDate === undefined || payload.joiningDate === '') payload.joiningDate = todayStr;
    if (payload.assignedDate === undefined || payload.assignedDate === '') payload.assignedDate = new Date().toISOString();
    payload.assignmentStatus = parseStatusByte(payload.assignmentStatus, 1);
    if (!payload.assignedByUserId) payload.assignedByUserId = 0;

    try {
      if (editingItem) {
        const id = getItemId(editingItem);
        await api.put(`${endpoint}/${id}`, payload);
      } else {
        await api.post(endpoint, payload);
      }
      setIsModalOpen(false);
      fetchData();
    } catch (err) {
      console.error('Error saving record:', err);
      let msg = null;
      if (err.response?.data?.message) {
        msg = err.response.data.message;
      } else if (err.response?.status === 403) {
        msg = 'Access Denied (403 Forbidden): Your user account role does not have Administrator permissions for this operation. Please log in again.';
      } else if (err.response?.status === 401) {
        msg = 'Session Expired (401 Unauthorized): Please log in again.';
      } else if (err.response?.data?.title) {
        msg = err.response.data.title;
      } else if (err.response?.data?.errors) {
        const errObj = err.response.data.errors;
        const firstKey = Object.keys(errObj)[0];
        msg = `${firstKey}: ${Array.isArray(errObj[firstKey]) ? errObj[firstKey][0] : errObj[firstKey]}`;
      } else if (typeof err.response?.data === 'string' && err.response.data.length < 200) {
        msg = err.response.data;
      } else if (err.message) {
        msg = err.message;
      }
      setFormError(msg || 'Failed to save record.');
    } finally {
      setSaving(false);
    }
  };

  const [searchQuery, setSearchQuery] = useState('');
  const [sortKey, setSortKey] = useState(idKey);
  const [sortOrder, setSortOrder] = useState('asc');

  useEffect(() => {
    setSortKey(idKey || (displayColumns[0]?.key || ''));
    setSearchQuery('');
    setSortOrder('asc');
  }, [idKey, endpoint, displayColumns]);

  const handleHeaderClick = (key) => {
    if (sortKey === key) {
      setSortOrder(prev => (prev === 'asc' ? 'desc' : 'asc'));
    } else {
      setSortKey(key);
      setSortOrder('asc');
    }
  };

  const processedData = React.useMemo(() => {
    let result = [...data];

    // Apply license filter from query params (for licenses module)
    const queryParams = new URLSearchParams(location.search);
    const filter = queryParams.get('filter');
    
    if (filter && endpoint === '/license') {
      const today = new Date();
      const thirtyDaysFromNow = new Date();
      thirtyDaysFromNow.setDate(today.getDate() + 30);
      
      if (filter === 'expired') {
        result = result.filter(item => {
          if (!item.expiryDate) return false;
          const expDate = new Date(item.expiryDate);
          return expDate < today;
        });
      } else if (filter === 'expiringsoon') {
        result = result.filter(item => {
          if (!item.expiryDate) return false;
          const expDate = new Date(item.expiryDate);
          return expDate >= today && expDate <= thirtyDaysFromNow;
        });
      }
    }

    if (searchQuery.trim()) {
      const q = searchQuery.toLowerCase().trim();
      result = result.filter(item =>
        displayColumns.some(col => {
          const val = item[col.key];
          if (val === null || val === undefined) return false;
          return String(val).toLowerCase().includes(q);
        })
      );
    }

    if (sortKey) {
      result.sort((a, b) => {
        let valA = a[sortKey];
        let valB = b[sortKey];

        if (valA === null || valA === undefined) return 1;
        if (valB === null || valB === undefined) return -1;

        let comp = 0;
        const numA = Number(valA);
        const numB = Number(valB);
        if (!isNaN(numA) && !isNaN(numB) && typeof valA !== 'boolean' && typeof valB !== 'boolean') {
          comp = numA - numB;
        } else {
          comp = String(valA).localeCompare(String(valB), undefined, { numeric: true, sensitivity: 'base' });
        }

        return sortOrder === 'asc' ? comp : -comp;
      });
    }

    return result;
  }, [data, searchQuery, sortKey, sortOrder, displayColumns, endpoint, location]);

  // Get row background color based on license expiry status
  const getRowBgColor = (item) => {
    if (endpoint === '/license' && item.expiryDate) {
      const today = new Date();
      const expDate = new Date(item.expiryDate);
      const thirtyDaysFromNow = new Date();
      thirtyDaysFromNow.setDate(today.getDate() + 30);
      
      if (expDate < today) {
        // Expired - red background
        return 'rgba(220, 38, 38, 0.15)'; // Light red
      } else if (expDate <= thirtyDaysFromNow) {
        // Expiring soon - yellow background
        return 'rgba(234, 179, 8, 0.15)'; // Light yellow
      }
    }
    return 'transparent';
  };

  return (
    <div className="fade-in">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h2 style={{ fontSize: '1.75rem', fontWeight: 'bold' }}>{title}</h2>
        {!readOnly && formFields.length > 0 && (
          <button onClick={openAddModal} className="btn btn-primary">
            + Add New
          </button>
        )}
      </div>

      {errorMsg && (
        <div style={{ background: 'var(--danger-bg)', color: 'var(--danger-text)', padding: '0.75rem 1.25rem', borderRadius: 'var(--border-radius-sm)', marginBottom: '1.5rem', border: '1px solid rgba(220, 38, 38, 0.2)' }}>
          {errorMsg}
        </div>
      )}

      {/* Search and Filter Controls */}
      <div style={{
        display: 'flex',
        flexWrap: 'wrap',
        gap: '1rem',
        alignItems: 'center',
        justifyContent: 'space-between',
        marginBottom: '1.25rem',
        background: 'var(--bg-surface)',
        padding: '0.85rem 1.25rem',
        borderRadius: 'var(--border-radius-sm)',
        border: '1px solid var(--border-color)'
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', flex: 1, minWidth: '240px' }}>
          <span style={{ fontSize: '1rem', color: 'var(--text-secondary)' }}>🔍</span>
          <input
            type="text"
            placeholder={`Search ${title.toLowerCase()}...`}
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            style={{
              flex: 1,
              padding: '0.5rem 0.85rem',
              borderRadius: '6px',
              border: '1px solid var(--border-color)',
              background: 'var(--bg-main)',
              color: 'var(--text-primary)',
              fontSize: '0.875rem'
            }}
          />
          {searchQuery && (
            <button
              onClick={() => setSearchQuery('')}
              className="btn btn-secondary"
              style={{ padding: '0.35rem 0.65rem', fontSize: '0.75rem' }}
            >
              Clear
            </button>
          )}
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', flexWrap: 'wrap' }}>
          <label style={{ fontSize: '0.85rem', fontWeight: '600', color: 'var(--text-secondary)', whiteSpace: 'nowrap' }}>
            Sort By:
          </label>
          <select
            value={sortKey}
            onChange={(e) => setSortKey(e.target.value)}
            style={{
              padding: '0.5rem 0.85rem',
              borderRadius: '6px',
              border: '1px solid var(--border-color)',
              background: 'var(--bg-main)',
              color: 'var(--text-primary)',
              fontSize: '0.875rem'
            }}
          >
            {displayColumns.map(col => (
              <option key={col.key} value={col.key}>
                {col.label}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className="table-wrapper glass-panel">
        <table ref={tableRef}>
          <thead>
            <tr>
              {displayColumns.map(col => {
                const isSorted = sortKey === col.key;
                return (
                  <th
                    key={col.key}
                    onClick={() => handleHeaderClick(col.key)}
                    style={{ cursor: 'pointer', userSelect: 'none' }}
                    title={`Click to sort by ${col.label}`}
                  >
                    <div style={{ display: 'flex', alignItems: 'center', gap: '0.35rem' }}>
                      <span>{col.label}</span>
                      {isSorted ? (
                        <span style={{ fontSize: '0.75rem', color: 'var(--primary-color)' }}>
                          {sortOrder === 'asc' ? '▲' : '▼'}
                        </span>
                      ) : (
                        <span style={{ fontSize: '0.75rem', opacity: 0.3 }}>⇅</span>
                      )}
                    </div>
                  </th>
                );
              })}
              {!readOnly && <th style={{ width: '150px' }}>Actions</th>}
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={displayColumns.length + (readOnly ? 0 : 1)} style={{ textAlign: 'center', padding: '2rem' }}>Loading...</td></tr>
            ) : processedData.length === 0 ? (
              <tr><td colSpan={displayColumns.length + (readOnly ? 0 : 1)} style={{ textAlign: 'center', padding: '2rem' }}>No matching records found.</td></tr>
            ) : (
              processedData.map((item, idx) => {
                const id = getItemId(item) || idx;
                const rowBgColor = getRowBgColor(item);
                return (
                  <tr key={id} id={`row-${id}`} className="table-row" style={{ opacity: 1, background: rowBgColor }}>
                    {displayColumns.map(col => (
                      <td key={`${id}-${col.key}`}>
                        {typeof item[col.key] === 'boolean' 
                          ? (item[col.key] ? 'Active' : 'Inactive') 
                          : (String(col.key).toLowerCase().includes('status') && item[col.key] === 1) ? 'Active'
                          : (String(col.key).toLowerCase().includes('status') && (item[col.key] === 0 || item[col.key] === 2)) ? 'Inactive'
                          : (String(col.key).toLowerCase().includes('status') && item[col.key] === 3) ? 'Pending'
                          : item[col.key] ?? '-'}
                      </td>
                    ))}
                    {!readOnly && (
                      <td>
                        <div style={{ display: 'flex', gap: '0.5rem' }}>
                          {formFields.length > 0 && (
                            <button onClick={() => openEditModal(item)} className="btn btn-secondary" style={{ padding: '0.25rem 0.75rem', fontSize: '0.875rem' }}>
                              Edit
                            </button>
                          )}
                          <button onClick={() => handleDelete(item)} className="btn btn-danger" style={{ padding: '0.25rem 0.75rem', fontSize: '0.875rem' }}>
                            Delete
                          </button>
                        </div>
                      </td>
                    )}
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>

      {/* Add / Edit Modal */}
      {isModalOpen && (
        <div style={{
          position: 'fixed',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          backgroundColor: 'rgba(15, 23, 42, 0.6)',
          backdropFilter: 'blur(4px)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          zIndex: 1000
        }}>
          <div className="card" style={{ width: '100%', maxWidth: '500px', padding: '2rem', background: 'var(--bg-surface)', minHeight: '600px', maxHeight: '85vh', overflowY: 'auto' }}>
            <h3 style={{ marginBottom: '1.5rem', fontSize: '1.25rem', fontWeight: 'bold' }}>
              {editingItem ? `Edit ${title.slice(0, -1)}` : `Add New ${title.slice(0, -1)}`}
            </h3>

            {formError && (
              <div style={{ background: 'var(--danger-bg)', color: 'var(--danger-text)', padding: '0.5rem 1rem', borderRadius: '6px', marginBottom: '1rem', fontSize: '0.85rem' }}>
                {formError}
              </div>
            )}

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
              {formFields.map(field => {
                if (typeof field.hideIf === 'function' && field.hideIf(formData)) {
                  return null;
                }
                const isRequired = typeof field.required === 'function' ? field.required(formData) : field.required;
                const isDependentField = field.dependsOn && field.filterKey;
                const parentValue = isDependentField ? formData[field.dependsOn] : null;
                const isDisabled = isDependentField && !parentValue;
                
                let filteredOptions = optionsMap[field.name] || [];
                if (isDependentField && parentValue && optionsMap[field.name]) {
                  filteredOptions = optionsMap[field.name].filter(opt => 
                    opt[field.filterKey] == parentValue
                  );
                }
                
                return (
                  <div key={field.name}>
                    <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: '600', color: 'var(--text-secondary)', marginBottom: '0.35rem' }}>
                      {field.label.toUpperCase()} {isRequired && '*'} {isDependentField && !parentValue && <span style={{color: 'var(--accent-primary)', fontSize: '0.75rem'}}>(Select {formFields.find(f => f.name === field.dependsOn)?.label} first)</span>}
                    </label>
                    {field.type === 'select' ? (
                      field.options ? (
                        <select
                          value={formData[field.name] !== undefined ? formData[field.name] : ''}
                          onChange={(e) => handleInputChange(field.name, e.target.value)}
                          required={isRequired}
                          disabled={isDisabled}
                          style={{ width: '100%', padding: '0.65rem 0.85rem', borderRadius: '8px', border: '1px solid var(--border-color)', background: isDisabled ? 'var(--bg-tertiary)' : 'var(--bg-main)', color: 'var(--text-primary)', fontSize: '0.9rem', opacity: isDisabled ? 0.6 : 1, cursor: isDisabled ? 'not-allowed' : 'pointer' }}
                        >
                          <option value="">-- Select {field.label} --</option>
                          {field.options.map(opt => (
                            <option key={typeof opt === 'object' ? opt.value : opt} value={typeof opt === 'object' ? opt.value : opt}>
                              {typeof opt === 'object' ? opt.label : opt}
                            </option>
                          ))}
                        </select>
                      ) : (
                        <CustomDropdown
                          value={formData[field.name] !== undefined ? formData[field.name] : ''}
                          onChange={(val) => handleInputChange(field.name, val)}
                          options={filteredOptions}
                          label={field.label}
                          disabled={isDisabled}
                          required={isRequired}
                          labelKey={field.labelKey}
                          valueKey={field.valueKey}
                        />
                      )
                    ) : (
                      <input
                        type={field.type || 'text'}
                        value={formData[field.name] !== undefined ? formData[field.name] : ''}
                        onChange={(e) => handleInputChange(field.name, e.target.value)}
                        required={isRequired}
                        style={{ width: '100%', padding: '0.65rem 0.85rem', borderRadius: '8px', border: '1px solid var(--border-color)', background: 'var(--bg-main)', color: 'var(--text-primary)', fontSize: '0.9rem' }}
                      />
                    )}
                  </div>
                );
              })}

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">
                  Cancel
                </button>
                <button type="submit" disabled={saving} className="btn btn-primary">
                  {saving ? 'Saving...' : (editingItem ? 'Save Changes' : 'Create')}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default CrudPage;
