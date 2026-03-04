import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import {
  CCard,
  CCardBody,
  CCardHeader,
  CRow,
  CCol,
  CButton,
  CFormInput,
  CFormLabel,
  CForm,
  CModal,
  CModalBody,
  CModalHeader,
  CModalTitle,
  CModalFooter,
  CTable,
  CTableHead,
  CTableRow,
  CTableHeaderCell,
  CTableBody,
  CTableDataCell,
  CPagination,
  CPaginationItem,
  CBadge,
  CSpinner,
  CAlert,
  CFormSelect,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPencil, cilTrash, cilPlus, cilWarning, cilLockLocked } from '@coreui/icons';
import { DocsComponents } from 'src/components';
import Pagination from 'src/components/Pagination';
import UserModal from 'src/components/UserModal';
import UserPasswordModal from 'src/components/UserPasswordModal';
import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL || 'http://localhost:5144';

const UserList = () => {
  const { t } = useTranslation();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [users, setUsers] = useState([]);
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10,
    total_count: 0,
    total_pages: 0,
    has_previous: false,
    has_next: false,
  });
  const [searchTerm, setSearchTerm] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [showPasswordModal, setShowPasswordModal] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);
  const [modalMode, setModalMode] = useState('create'); // 'create' or 'edit'
  const [successMessage, setSuccessMessage] = useState('');

  // Get auth token from localStorage
  const getToken = () => localStorage.getItem('auth_token') || '';
  const getCompanyId = () => localStorage.getItem('company_id') || '';

  const fetchUsers = async (page = 1, search = '') => {
    setLoading(true);
    setError(null);
    try {
      const token = getToken();
      const companyId = getCompanyId();
      const response = await axios.get(`${API_BASE_URL}/user`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'x-company': companyId,
        },
        params: {
          page,
          pageSize: 10,
          searchTerm: search,
        },
      });

      setUsers(response.data.users || []);
      setPagination({
        page: response.data.page,
        pageSize: response.data.pageSize,
        total_count: response.data.total_count,
        total_pages: response.data.total_pages,
        has_previous: response.data.has_previous,
        has_next: response.data.has_next,
      });
    } catch (err) {
      setError(err.response?.data?.message || t('users.errors.fetchError') || 'Error fetching users');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleSearch = (e) => {
    e.preventDefault();
    fetchUsers(1, searchTerm);
  };

  const handlePageChange = (newPage) => {
    fetchUsers(newPage, searchTerm);
  };

  const handleCreate = () => {
    setModalMode('create');
    setSelectedUser(null);
    setShowModal(true);
  };

  const handleEdit = (user) => {
    setModalMode('edit');
    setSelectedUser(user);
    setShowModal(true);
  };

  const handleDelete = async (user) => {
    if (window.confirm(t('users.confirmDelete') || 'Are you sure you want to delete this user?')) {
      try {
        const token = getToken();
        const companyId = getCompanyId();
        await axios.delete(`${API_BASE_URL}/user/${user.id}`, {
          headers: {
            Authorization: `Bearer ${token}`,
            'x-company': companyId,
          },
        });
        setSuccessMessage(t('users.deletedSuccessfully') || 'User deleted successfully');
        setTimeout(() => setSuccessMessage(''), 3000);
        fetchUsers(pagination.page, searchTerm);
      } catch (err) {
        setError(err.response?.data?.message || t('users.errors.deleteError') || 'Error deleting user');
        setTimeout(() => setError(''), 3000);
      }
    }
  };

  const handleChangePassword = (user) => {
    setSelectedUser(user);
    setShowPasswordModal(true);
  };

  const handleModalSave = () => {
    fetchUsers(pagination.page, searchTerm);
    setShowModal(false);
    setSuccessMessage(
      modalMode === 'create'
        ? t('users.createdSuccessfully') || 'User created successfully'
        : t('users.updatedSuccessfully') || 'User updated successfully'
    );
    setTimeout(() => setSuccessMessage(''), 3000);
  };

  const handlePasswordChange = () => {
    setShowPasswordModal(false);
    setSuccessMessage(t('users.passwordChangedSuccessfully') || 'Password changed successfully');
    setTimeout(() => setSuccessMessage(''), 3000);
  };

  return (
    <CRow>
      <CCol xs={12}>
        <DocsComponents href="components/table/" />
        <CCard className="mb-4">
          <CCardHeader>
            <strong>{t('users.title') || 'User Management'}</strong>
          </CCardHeader>
          <CCardBody>
            {error && (
              <CAlert color="danger" dismissible onClose={() => setError(null)}>
                {error}
              </CAlert>
            )}
            {successMessage && (
              <CAlert color="success" dismissible onClose={() => setSuccessMessage(null)}>
                {successMessage}
              </CAlert>
            )}

            {/* Search and Create */}
            <CRow className="mb-3">
              <CCol md={6}>
                <CForm onSubmit={handleSearch} className="d-flex gap-2">
                  <CFormInput
                    placeholder={t('users.searchPlaceholder') || 'Search by name or email...'}
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    style={{ maxWidth: '300px' }}
                  />
                  <CButton color="info" type="submit">
                    {t('common.search') || 'Search'}
                  </CButton>
                </CForm>
              </CCol>
              <CCol md={6} className="text-end">
                <CButton color="primary" onClick={handleCreate}>
                  <CIcon icon={cilPlus} className="me-2" />
                  {t('users.createNew') || 'Create New User'}
                </CButton>
              </CCol>
            </CRow>

            {/* Users Table */}
            {loading ? (
              <div className="text-center py-5">
                <CSpinner color="primary" />
              </div>
            ) : (
              <>
                <CTable hover responsive>
                  <CTableHead>
                    <CTableRow>
                      <CTableHeaderCell>{t('users.name') || 'Name'}</CTableHeaderCell>
                      <CTableHeaderCell>{t('users.email') || 'Email'}</CTableHeaderCell>
                      <CTableHeaderCell>{t('users.companies') || 'Companies'}</CTableHeaderCell>
                      <CTableHeaderCell>{t('users.created') || 'Created'}</CTableHeaderCell>
                      <CTableHeaderCell className="text-center">
                        {t('common.actions') || 'Actions'}
                      </CTableHeaderCell>
                    </CTableRow>
                  </CTableHead>
                  <CTableBody>
                    {users.length === 0 ? (
                      <CTableRow>
                        <CTableDataCell colSpan="5" className="text-center">
                          {t('users.noUsers') || 'No users found'}
                        </CTableDataCell>
                      </CTableRow>
                    ) : (
                      users.map((user) => (
                        <CTableRow key={user.id}>
                          <CTableDataCell>{user.name}</CTableDataCell>
                          <CTableDataCell>{user.email}</CTableDataCell>
                          <CTableDataCell>
                            {user.companies && user.companies.length > 0 ? (
                              user.companies.map((c, idx) => (
                                <CBadge color="info" className="me-1 mb-1" key={idx}>
                                  {c.company_name} ({c.role_name})
                                </CBadge>
                              ))
                            ) : (
                              <CBadge color="secondary">No companies</CBadge>
                            )}
                          </CTableDataCell>
                          <CTableDataCell>
                            {new Date(user.created).toLocaleDateString()}
                          </CTableDataCell>
                          <CTableDataCell className="text-center">
                            <CButton
                              color="info"
                              size="sm"
                              className="me-1"
                              onClick={() => handleEdit(user)}
                              title={t('common.edit') || 'Edit'}
                            >
                              <CIcon icon={cilPencil} />
                            </CButton>
                            <CButton
                              color="warning"
                              size="sm"
                              className="me-1"
                              onClick={() => handleChangePassword(user)}
                              title={t('users.changePassword') || 'Change Password'}
                            >
                              <CIcon icon={cilLockLocked} />
                            </CButton>
                            <CButton
                              color="danger"
                              size="sm"
                              onClick={() => handleDelete(user)}
                              title={t('common.delete') || 'Delete'}
                            >
                              <CIcon icon={cilTrash} />
                            </CButton>
                          </CTableDataCell>
                        </CTableRow>
                      ))
                    )}
                  </CTableBody>
                </CTable>

                {/* Pagination */}
                {pagination.total_pages > 1 && (
                  <Pagination
                    currentPage={pagination.page}
                    totalPages={pagination.total_pages}
                    onPageChange={handlePageChange}
                    hasNext={pagination.has_next}
                    hasPrevious={pagination.has_previous}
                  />
                )}
              </>
            )}
          </CCardBody>
        </CCard>
      </CCol>

      {/* User Modal */}
      <UserModal
        visible={showModal}
        onClose={() => setShowModal(false)}
        onSave={handleModalSave}
        mode={modalMode}
        user={selectedUser}
      />

      {/* Password Change Modal */}
      <UserPasswordModal
        visible={showPasswordModal}
        onClose={() => setShowPasswordModal(false)}
        onSave={handlePasswordChange}
        userId={selectedUser?.id}
      />
    </CRow>
  );
};

export default UserList;
