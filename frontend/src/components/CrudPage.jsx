import React, { useState, useEffect, useRef } from 'react';
import anime from 'animejs';
import api from '../services/api';

const CrudPage = ({ title, endpoint: endpointProp, columns: columnsProp, config }) => {
  const endpoint = config?.endpoint || endpointProp;
  const columns = config?.columns || columnsProp || [];
  const idKey = config?.idKey || 'id';
  const formFields = config?.formFields || [];
  const readOnly = config?.readOnly || false;

  const [data, setData] = useState([]);
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

  const fetchData = async () => {
    setLoading(true);
    setErrorMsg('');
    try {
      const response = await api.get(endpoint);
      const items = Array.isArray(response.data) ? response.data : (response.data ? [response.data] : []);
      setData(items);
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
      initialForm[f.name] = '';
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
    setFormData(prev => ({ ...prev, [fieldName]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setFormError('');

    // Format payload before posting
    const payload = { ...formData };
    formFields.forEach(f => {
      if ((f.type === 'number' || f.type === 'select') && payload[f.name] !== '' && payload[f.name] !== null && payload[f.name] !== undefined) {
        payload[f.name] = Number(payload[f.name]);
      }
    });

    // Provide default required fields for backend DTOs if omitted
    const todayStr = new Date().toISOString().split('T')[0];
    if (payload.purchaseDate === undefined || payload.purchaseDate === '') payload.purchaseDate = todayStr;
    if (payload.licenseStatus === undefined || payload.licenseStatus === '') payload.licenseStatus = 1;
    if (payload.joiningDate === undefined || payload.joiningDate === '') payload.joiningDate = todayStr;
    if (payload.assignedDate === undefined || payload.assignedDate === '') payload.assignedDate = new Date().toISOString();
    if (payload.assignmentStatus === undefined || payload.assignmentStatus === '') payload.assignmentStatus = 1;
    if (!payload.assignedByUserId) payload.assignedByUserId = 1;

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
      if (err.response?.status === 403) {
        msg = 'Access Denied (403 Forbidden): Your user account role does not have Administrator permissions for this operation. Please log in again.';
      } else if (err.response?.status === 401) {
        msg = 'Session Expired (401 Unauthorized): Please log in again.';
      } else if (err.response?.data?.message) {
        msg = err.response.data.message;
      } else if (err.response?.data?.title) {
        msg = err.response.data.title;
      } else if (err.response?.data?.errors) {
        const errObj = err.response.data.errors;
        const firstKey = Object.keys(errObj)[0];
        msg = `${firstKey}: ${Array.isArray(errObj[firstKey]) ? errObj[firstKey][0] : errObj[firstKey]}`;
      } else if (typeof err.response?.data === 'string' && err.response.data.length < 200) {
        msg = err.response.data;
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
    setSortKey(idKey || (columns[0]?.key || ''));
    setSearchQuery('');
    setSortOrder('asc');
  }, [idKey, endpoint]);

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

    if (searchQuery.trim()) {
      const q = searchQuery.toLowerCase().trim();
      result = result.filter(item =>
        columns.some(col => {
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
  }, [data, searchQuery, sortKey, sortOrder, columns]);

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
            {columns.map(col => (
              <option key={col.key} value={col.key}>
                {col.label}
              </option>
            ))}
          </select>

          <button
            onClick={() => setSortOrder(prev => (prev === 'asc' ? 'desc' : 'asc'))}
            className="btn btn-secondary"
            style={{ padding: '0.5rem 0.85rem', fontSize: '0.875rem', display: 'flex', alignItems: 'center', gap: '0.4rem' }}
          >
            <span>{sortOrder === 'asc' ? '▲ Ascending' : '▼ Descending'}</span>
          </button>
        </div>
      </div>

      <div className="table-wrapper glass-panel">
        <table ref={tableRef}>
          <thead>
            <tr>
              {columns.map(col => {
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
              <tr><td colSpan={columns.length + (readOnly ? 0 : 1)} style={{ textAlign: 'center', padding: '2rem' }}>Loading...</td></tr>
            ) : processedData.length === 0 ? (
              <tr><td colSpan={columns.length + (readOnly ? 0 : 1)} style={{ textAlign: 'center', padding: '2rem' }}>No matching records found.</td></tr>
            ) : (
              processedData.map((item, idx) => {
                const id = getItemId(item) || idx;
                return (
                  <tr key={id} id={`row-${id}`} className="table-row" style={{ opacity: 1 }}>
                    {columns.map(col => (
                      <td key={`${id}-${col.key}`}>
                        {typeof item[col.key] === 'boolean' 
                          ? (item[col.key] ? 'Active' : 'Inactive') 
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
          <div className="card" style={{ width: '100%', maxWidth: '500px', padding: '2rem', background: 'var(--bg-surface)' }}>
            <h3 style={{ marginBottom: '1.5rem', fontSize: '1.25rem', fontWeight: 'bold' }}>
              {editingItem ? `Edit ${title.slice(0, -1)}` : `Add New ${title.slice(0, -1)}`}
            </h3>

            {formError && (
              <div style={{ background: 'var(--danger-bg)', color: 'var(--danger-text)', padding: '0.5rem 1rem', borderRadius: '6px', marginBottom: '1rem', fontSize: '0.85rem' }}>
                {formError}
              </div>
            )}

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
              {formFields.map(field => (
                <div key={field.name}>
                  <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: '600', color: 'var(--text-secondary)', marginBottom: '0.35rem' }}>
                    {field.label.toUpperCase()} {field.required && '*'}
                  </label>
                  {field.type === 'select' ? (
                    <select
                      value={formData[field.name] !== undefined ? formData[field.name] : ''}
                      onChange={(e) => handleInputChange(field.name, e.target.value)}
                      required={field.required}
                      style={{ width: '100%', padding: '0.65rem 0.85rem', borderRadius: '8px', border: '1px solid var(--border-color)', background: 'var(--bg-main)', color: 'var(--text-primary)', fontSize: '0.9rem' }}
                    >
                      <option value="">-- Select {field.label} --</option>
                      {(optionsMap[field.name] || []).map(opt => (
                        <option key={opt[field.valueKey]} value={opt[field.valueKey]}>
                          {opt[field.labelKey] || opt.name || opt[field.valueKey]} (ID: {opt[field.valueKey]})
                        </option>
                      ))}
                    </select>
                  ) : (
                    <input
                      type={field.type || 'text'}
                      value={formData[field.name] !== undefined ? formData[field.name] : ''}
                      onChange={(e) => handleInputChange(field.name, e.target.value)}
                      required={field.required}
                      style={{ width: '100%', padding: '0.65rem 0.85rem', borderRadius: '8px', border: '1px solid var(--border-color)', background: 'var(--bg-main)', color: 'var(--text-primary)', fontSize: '0.9rem' }}
                    />
                  )}
                </div>
              ))}

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
