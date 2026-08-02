import React, { useContext, useState } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { AuthContext } from './contexts/AuthContext';
import Login from './pages/Login';
import Sidebar from './components/Sidebar';
import Navbar from './components/Navbar';
import Dashboard from './pages/Dashboard';
import CrudPage from './components/CrudPage';

const ProtectedRoute = ({ children, requiredRole, allowedRoles }) => {
  const { user, loading } = useContext(AuthContext);
  
  if (loading) return <div style={{ padding: '2rem' }}>Loading...</div>;
  if (!user) return <Navigate to="/login" replace />;
  
  const userRole = user.role?.toLowerCase() || '';

  if (allowedRoles && Array.isArray(allowedRoles)) {
    const hasRole = allowedRoles.some(r => userRole.includes(r.toLowerCase()));
    if (!hasRole) return <Navigate to="/" replace />;
  } else if (requiredRole && !userRole.includes(requiredRole.toLowerCase())) {
    return <Navigate to="/" replace />;
  }

  return children;
};

// Entity Definitions for CRUD Pages aligned with API DTOs
const entityConfigs = {
  departments: {
    idKey: 'departmentId',
    endpoint: '/departments',
    columns: [
      { key: 'departmentId', label: 'ID' },
      { key: 'departmentName', label: 'Department Name' },
      { key: 'description', label: 'Description' },
      { key: 'isActive', label: 'Is Active' }
    ],
    formFields: [
      { name: 'departmentName', label: 'Department Name', required: true },
      { name: 'description', label: 'Description' },
      { 
        name: 'isActive', 
        label: 'Is Active', 
        type: 'select', 
        options: [
          { value: true, label: 'Active' },
          { value: false, label: 'Inactive' }
        ],
        defaultValue: true 
      }
    ]
  },
  employees: {
    idKey: 'employeeId',
    endpoint: '/employees',
    columns: [
      { key: 'employeeId', label: 'ID' },
      { key: 'employeeCode', label: 'Code' },
      { key: 'fullName', label: 'Name' },
      { key: 'email', label: 'Email' },
      { key: 'departmentName', label: 'Department' },
      { key: 'designation', label: 'Designation' }
    ],
    formFields: [
      { name: 'employeeCode', label: 'Employee Code', required: true },
      { name: 'firstName', label: 'First Name', required: true },
      { name: 'lastName', label: 'Last Name', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
      { name: 'phone', label: 'Phone Number', required: false },
      { name: 'departmentId', label: 'Department', type: 'select', endpoint: '/departments', valueKey: 'departmentId', labelKey: 'departmentName', required: true },
      { name: 'designation', label: 'Designation', required: true },
      { name: 'joiningDate', label: 'Joining Date', type: 'date', required: true, defaultValue: () => new Date().toISOString().split('T')[0] },
      { name: 'employmentStatus', label: 'Employment Status', type: 'select', options: ['Active', 'Inactive', 'On Leave', 'Terminated'], required: true, defaultValue: 'Active' }
    ]
  },
  licenses: {
    idKey: 'licenseId',
    endpoint: '/license',
    columns: [
      { key: 'licenseId', label: 'ID' },
      { key: 'softwareName', label: 'Software' },
      { key: 'licenseKey', label: 'License Key' },
      { key: 'licenseType', label: 'Type' },
      { key: 'purchaseDate', label: 'Purchase Date' },
      { key: 'expiryDate', label: 'Expiry Date' },
      { key: 'seats', label: 'Seats' },
      { key: 'purchaseCost', label: 'Cost ($)' }
    ],
    formFields: [
      { name: 'softwareId', label: 'Software Application', type: 'select', endpoint: '/software', valueKey: 'softwareId', labelKey: 'softwareName', required: true },
      { name: 'licenseKey', label: 'License Key', required: true },
      { 
        name: 'licenseType', 
        label: 'License Type', 
        type: 'select', 
        options: ['Perpetual', 'Subscription', 'Trial', 'Volume', 'OEM', 'Non-Expiry'],
        required: true 
      },
      { 
        name: 'purchaseDate', 
        label: 'Purchase Date', 
        type: 'date', 
        required: true, 
        defaultValue: () => new Date().toISOString().split('T')[0] 
      },
      { 
        name: 'expiryDate', 
        label: 'Expiry Date', 
        type: 'date', 
        required: (formData) => formData.licenseType !== 'Non-Expiry', 
        hideIf: (formData) => formData.licenseType === 'Non-Expiry' 
      },
      { name: 'seats', label: 'Seats', type: 'number', required: true },
      { name: 'purchaseCost', label: 'Purchase Cost', type: 'number', required: true }
    ]
  },
  assignments: {
    idKey: 'assignmentId',
    endpoint: '/licenseassignment',
    columns: [
      { key: 'assignmentId', label: 'ID' },
      { key: 'employeeName', label: 'Employee' },
      { key: 'licenseKey', label: 'License Key' },
      { key: 'assignedDate', label: 'Assigned Date' },
      { key: 'assignedByUsername', label: 'Assigned By' }
    ],
    formFields: [
      { name: 'softwareId', label: 'Software', type: 'select', endpoint: '/software', valueKey: 'softwareId', labelKey: 'softwareName', required: true },
      { name: 'licenseId', label: 'License Key', type: 'select', endpoint: '/license', valueKey: 'licenseId', labelKey: 'licenseKey', required: true, dependsOn: 'softwareId', filterKey: 'softwareId' },
      { name: 'employeeId', label: 'Employee', type: 'select', endpoint: '/employees', valueKey: 'employeeId', labelKey: 'fullName', required: true },
      { name: 'remarks', label: 'Remarks', required: false }
    ]
  },
  assigned: {
    idKey: 'assignmentId',
    endpoint: '/licenseassignment/my-assignments',
    readOnly: true,
    columns: [
      { key: 'assignmentId', label: 'ID' },
      { key: 'softwareName', label: 'Software Name' },
      { key: 'licenseKey', label: 'License Key' },
      { key: 'assignedDate', label: 'Assigned Date' },
      { key: 'assignmentStatus', label: 'Status' }
    ],
    formFields: []
  },
  roles: {
    idKey: 'roleId',
    endpoint: '/roles',
    columns: [
      { key: 'roleId', label: 'ID' },
      { key: 'roleName', label: 'Role Name' },
      { key: 'description', label: 'Description' }
    ],
    formFields: [
      { name: 'roleName', label: 'Role Name', required: true },
      { name: 'description', label: 'Description' }
    ]
  },
  software: {
    idKey: 'softwareId',
    endpoint: '/software',
    columns: [
      { key: 'softwareId', label: 'ID' },
      { key: 'softwareName', label: 'Software Name' },
      { key: 'vendorName', label: 'Vendor' },
      { key: 'version', label: 'Version' },
      { key: 'category', label: 'Category' }
    ],
    formFields: [
      { name: 'vendorId', label: 'Vendor', type: 'select', endpoint: '/vendors', valueKey: 'vendorId', labelKey: 'vendorName', required: true },
      { name: 'softwareName', label: 'Software Name', required: true },
      { name: 'version', label: 'Version', required: true },
      { name: 'category', label: 'Category' }
    ]
  },
  users: {
    idKey: 'userId',
    endpoint: '/users',
    columns: [
      { key: 'userId', label: 'User ID' },
      { key: 'employeeId', label: 'Employee ID' },
      { key: 'employeeName', label: 'Employee Name' },
      { key: 'roleId', label: 'Role ID' },
      { key: 'roleName', label: 'Role Name' },
      { key: 'username', label: 'Username' },
      { key: 'email', label: 'Email' },
      { key: 'lastLogin', label: 'Last Login' },
      { key: 'isActive', label: 'Is Active' },
      { key: 'createdAt', label: 'Created At' }
    ],
    formFields: [
      {
        name: 'employeeId',
        label: 'Employee',
        type: 'select',
        endpoint: '/employees',
        valueKey: 'employeeId',
        labelKey: 'fullName',
        required: true,
        autofill: {
          email: 'email',
          username: 'email'
        }
      },
      { name: 'username', label: 'Username', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
      { name: 'password', label: 'Password', type: 'password', required: (formData) => !formData.userId },
      { name: 'roleId', label: 'Role', type: 'select', endpoint: '/roles', valueKey: 'roleId', labelKey: 'roleName', required: true },
      { 
        name: 'isActive', 
        label: 'Status', 
        type: 'select', 
        options: [
          { value: true, label: 'Active' },
          { value: false, label: 'Inactive' }
        ],
        defaultValue: true 
      }
    ]
  },
  vendors: {
    idKey: 'vendorId',
    endpoint: '/vendors',
    columns: [
      { key: 'vendorId', label: 'ID' },
      { key: 'vendorName', label: 'Vendor Name' },
      { key: 'contactPerson', label: 'Contact Person' },
      { key: 'email', label: 'Email' },
      { key: 'phone', label: 'Phone' }
    ],
    formFields: [
      { name: 'vendorName', label: 'Vendor Name', required: true },
      { name: 'contactPerson', label: 'Contact Person' },
      { name: 'email', label: 'Email', type: 'email' },
      { name: 'phone', label: 'Phone' }
    ]
  },
  auditlogs: {
    idKey: 'auditLogId',
    endpoint: '/auditlog',
    columns: [
      { key: 'auditLogId', label: 'Log ID' },
      { key: 'username', label: 'User' },
      { key: 'action', label: 'Action' },
      { key: 'tableName', label: 'Table' },
      { key: 'recordId', label: 'Record ID' },
      { key: 'description', label: 'Details' },
      { key: 'actionDate', label: 'Date & Time' }
    ],
    readOnly: true
  }
};

function App() {
  const { user } = useContext(AuthContext);
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);

  return (
    <div className="app-container">
      {user && <Navbar isSidebarCollapsed={isSidebarCollapsed} setIsSidebarCollapsed={setIsSidebarCollapsed} />}
      
      <div className={user ? "main-layout" : ""}>
        {user && <Sidebar isCollapsed={isSidebarCollapsed} />}
        
        <div className={`content-container ${isSidebarCollapsed ? 'collapsed' : ''}`} style={{ padding: user ? '2rem' : '0' }}>
          <Routes>
            <Route path="/login" element={!user ? <Login /> : <Navigate to="/" replace />} />
            
            {/* Protected Routes */}
            <Route path="/" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
            <Route path="/departments" element={<ProtectedRoute><CrudPage title="Departments" endpoint="/departments" config={entityConfigs.departments} /></ProtectedRoute>} />
            <Route path="/employees" element={<ProtectedRoute><CrudPage title="Employees" config={entityConfigs.employees} /></ProtectedRoute>} />
            <Route path="/software" element={<ProtectedRoute><CrudPage title="Software" config={entityConfigs.software} /></ProtectedRoute>} />
            <Route path="/licenses" element={<ProtectedRoute allowedRoles={['admin', 'manager']}><CrudPage title="Licenses" config={entityConfigs.licenses} /></ProtectedRoute>} />
            <Route path="/assignments" element={<ProtectedRoute allowedRoles={['admin', 'manager']}><CrudPage title="License Assignments" config={entityConfigs.assignments} /></ProtectedRoute>} />
            <Route path="/vendors" element={<ProtectedRoute requiredRole="admin"><CrudPage title="Vendors" endpoint="/vendors" config={entityConfigs.vendors} /></ProtectedRoute>} />
            <Route path="/roles" element={<ProtectedRoute requiredRole="admin"><CrudPage title="Roles" endpoint="/roles" config={entityConfigs.roles} /></ProtectedRoute>} />
            <Route path="/users" element={<ProtectedRoute requiredRole="admin"><CrudPage title="Users" endpoint="/users" config={entityConfigs.users} /></ProtectedRoute>} />
            <Route path="/assigned" element={<ProtectedRoute><CrudPage title="My Assigned Software" endpoint="/licenseassignment/my-assignments" config={entityConfigs.assigned} /></ProtectedRoute>} />
            <Route path="/auditlogs" element={<ProtectedRoute requiredRole="admin"><CrudPage title="Security & Audit Logs" endpoint="/auditlog" config={entityConfigs.auditlogs} /></ProtectedRoute>} />
            
            {/* Catch all */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </div>
      </div>
    </div>
  );
}

export default App;
