import React, { useContext } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { AuthContext } from './contexts/AuthContext';
import Login from './pages/Login';
import Sidebar from './components/Sidebar';
import Navbar from './components/Navbar';
import Dashboard from './pages/Dashboard';
import CrudPage from './components/CrudPage';

const ProtectedRoute = ({ children }) => {
  const { user, loading } = useContext(AuthContext);
  
  if (loading) return <div style={{ padding: '2rem' }}>Loading...</div>;
  if (!user) return <Navigate to="/login" replace />;
  
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
      { key: 'description', label: 'Description' }
    ],
    formFields: [
      { name: 'departmentName', label: 'Department Name', required: true },
      { name: 'description', label: 'Description' }
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
      { name: 'departmentId', label: 'Department', type: 'select', endpoint: '/departments', valueKey: 'departmentId', labelKey: 'departmentName', required: true },
      { name: 'designation', label: 'Designation' }
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
      { name: 'employeeId', label: 'Employee', type: 'select', endpoint: '/employees', valueKey: 'employeeId', labelKey: 'fullName', required: true }
    ]
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
      { key: 'userId', label: 'ID' },
      { key: 'username', label: 'Username' },
      { key: 'email', label: 'Email' },
      { key: 'roleName', label: 'Role' },
      { key: 'employeeName', label: 'Employee' }
    ],
    formFields: [
      { name: 'username', label: 'Username', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
      { name: 'password', label: 'Password', type: 'password', required: true },
      { name: 'roleId', label: 'Role', type: 'select', endpoint: '/roles', valueKey: 'roleId', labelKey: 'roleName', required: true },
      { name: 'employeeId', label: 'Employee', type: 'select', endpoint: '/employees', valueKey: 'employeeId', labelKey: 'fullName', required: true }
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
      { key: 'auditLogId', label: 'ID' },
      { key: 'username', label: 'User' },
      { key: 'action', label: 'Action' },
      { key: 'tableName', label: 'Table' },
      { key: 'actionDate', label: 'Date' }
    ],
    readOnly: true
  }
};

function App() {
  const { user } = useContext(AuthContext);

  return (
    <div className="app-container">
      {user && <Navbar />}
      
      <div className={user ? "main-layout" : ""}>
        {user && <Sidebar />}
        
        <div className="content-container" style={{ padding: user ? '2rem' : '0' }}>
          <Routes>
            <Route path="/login" element={!user ? <Login /> : <Navigate to="/" replace />} />
            
            {/* Protected Routes */}
            <Route path="/" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
            <Route path="/departments" element={<ProtectedRoute><CrudPage title="Departments" config={entityConfigs.departments} /></ProtectedRoute>} />
            <Route path="/employees" element={<ProtectedRoute><CrudPage title="Employees" config={entityConfigs.employees} /></ProtectedRoute>} />
            <Route path="/licenses" element={<ProtectedRoute><CrudPage title="Licenses" config={entityConfigs.licenses} /></ProtectedRoute>} />
            <Route path="/assignments" element={<ProtectedRoute><CrudPage title="License Assignments" config={entityConfigs.assignments} /></ProtectedRoute>} />
            <Route path="/roles" element={<ProtectedRoute><CrudPage title="Roles" config={entityConfigs.roles} /></ProtectedRoute>} />
            <Route path="/software" element={<ProtectedRoute><CrudPage title="Software" config={entityConfigs.software} /></ProtectedRoute>} />
            <Route path="/users" element={<ProtectedRoute><CrudPage title="Users" config={entityConfigs.users} /></ProtectedRoute>} />
            <Route path="/vendors" element={<ProtectedRoute><CrudPage title="Vendors" config={entityConfigs.vendors} /></ProtectedRoute>} />
            <Route path="/auditlogs" element={<ProtectedRoute><CrudPage title="Audit Logs" config={entityConfigs.auditlogs} /></ProtectedRoute>} />
            
            {/* Catch all */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </div>
      </div>
    </div>
  );
}

export default App;
