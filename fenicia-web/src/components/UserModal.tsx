import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import {
  CModal,
  CModalBody,
  CModalHeader,
  CModalTitle,
  CModalFooter,
  CButton,
  CForm,
  CFormInput,
  CFormLabel,
  CFormCheck,
  CSpinner,
  CAlert,
  CRow,
  CCol,
  CCard,
} from '@coreui/react';
import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const UserModal = ({ visible, onClose, onSave, mode, user }) => {
  const { t } = useTranslation();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [companies, setCompanies] = useState([]);
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    password: '',
    companiesRoles: [],
  });

  const getToken = () => localStorage.getItem('token') || '';
  const getCompanyId = () => localStorage.getItem('companyId') || '';

  // Fetch companies on mount
  useEffect(() => {
    if (visible) {
      fetchCompanies();
      if (mode === 'edit' && user) {
        setFormData({
          name: user.name || '',
          email: user.email || '',
          password: '',
          companiesRoles: user.companies?.map(c => ({
            companyId: c.company_id,
            roleId: c.role_id,
          })) || [],
        });
      } else {
        setFormData({
          name: '',
          email: '',
          password: '',
          companiesRoles: [],
        });
      }
    }
  }, [visible, mode, user]);

  const fetchCompanies = async () => {
    try {
      const token = getToken();
      const response = await axios.get(`${API_BASE_URL}/company`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'x-company': getCompanyId(),
        },
      });
      setCompanies(response.data || []);
    } catch (err) {
      console.error('Error fetching companies:', err);
    }
  };

  const fetchRoles = async () => {
    try {
      const token = getToken();
      const response = await axios.get(`${API_BASE_URL}/role`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'x-company': getCompanyId(),
        },
      });
      return response.data || [];
    } catch (err) {
      console.error('Error fetching roles:', err);
      return [];
    }
  };

  const [roles, setRoles] = useState([]);

  useEffect(() => {
    if (visible) {
      fetchRoles().then(setRoles);
    }
  }, [visible]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleCompanyRoleToggle = (companyId, roleId) => {
    const exists = formData.companiesRoles.find(
      (cr) => cr.companyId === companyId && cr.roleId === roleId
    );

    if (exists) {
      setFormData((prev) => ({
        ...prev,
        companiesRoles: prev.companiesRoles.filter(
          (cr) => !(cr.companyId === companyId && cr.roleId === roleId)
        ),
      }));
    } else {
      setFormData((prev) => ({
        ...prev,
        companiesRoles: [...prev.companiesRoles, { companyId, roleId }],
      }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const token = getToken();
      const payload = {
        name: formData.name,
        email: formData.email,
      };

      if (mode === 'create') {
        payload.password = formData.password;
      }

      if (formData.companiesRoles.length > 0) {
        payload.companiesRoles = formData.companiesRoles;
      }

      if (mode === 'create') {
        await axios.post(`${API_BASE_URL}/user`, payload, {
          headers: {
            Authorization: `Bearer ${token}`,
            'x-company': getCompanyId(),
          },
        });
      } else {
        await axios.patch(`${API_BASE_URL}/user/${user.id}`, payload, {
          headers: {
            Authorization: `Bearer ${token}`,
            'x-company': getCompanyId(),
          },
        });
      }

      onSave();
    } catch (err) {
      setError(err.response?.data?.message || t('users.errors.saveError') || 'Error saving user');
    } finally {
      setLoading(false);
    }
  };

  return (
    <CModal visible={visible} onClose={onClose} size="lg">
      <CModalHeader>
        <CModalTitle>
          {mode === 'create'
            ? t('users.createTitle') || 'Create New User'
            : t('users.editTitle') || 'Edit User'}
        </CModalTitle>
      </CModalHeader>
      <CModalBody>
        {error && (
          <CAlert color="danger" dismissible onClose={() => setError(null)}>
            {error}
          </CAlert>
        )}

        <CForm onSubmit={handleSubmit}>
          <CRow className="mb-3">
            <CCol md={6}>
              <CFormLabel htmlFor="name">{t('users.name') || 'Name'}</CFormLabel>
              <CFormInput
                id="name"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                required
              />
            </CCol>
            <CCol md={6}>
              <CFormLabel htmlFor="email">{t('users.email') || 'Email'}</CFormLabel>
              <CFormInput
                id="email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleInputChange}
                required
              />
            </CCol>
          </CRow>

          {mode === 'create' && (
            <CRow className="mb-3">
              <CCol md={12}>
                <CFormLabel htmlFor="password">
                  {t('users.password') || 'Password'}
                </CFormLabel>
                <CFormInput
                  id="password"
                  name="password"
                  type="password"
                  value={formData.password}
                  onChange={handleInputChange}
                  required
                  minLength={6}
                />
              </CCol>
            </CRow>
          )}

          <CRow className="mb-3">
            <CCol md={12}>
              <CFormLabel>{t('users.companiesAndRoles') || 'Companies and Roles'}</CFormLabel>
              <div
                style={{
                  maxHeight: '300px',
                  overflowY: 'auto',
                  border: '1px solid #d8dbe2',
                  borderRadius: '4px',
                  padding: '10px',
                }}
              >
                {companies.length === 0 ? (
                  <p className="text-muted">{t('users.noCompanies') || 'No companies available'}</p>
                ) : (
                  companies.map((company) => (
                    <CCard className="mb-2 p-2" key={company.id}>
                      <CFormCheck
                        id={`company-${company.id}`}
                        label={company.name}
                        checked={formData.companiesRoles.some((cr) => cr.companyId === company.id)}
                        onChange={() => {
                          // If already selected, remove all roles for this company
                          if (formData.companiesRoles.some((cr) => cr.companyId === company.id)) {
                            setFormData((prev) => ({
                              ...prev,
                              companiesRoles: prev.companiesRoles.filter(
                                (cr) => cr.companyId !== company.id
                              ),
                            }));
                          } else {
                            // Add with first role
                            const firstRole = roles[0];
                            if (firstRole) {
                              setFormData((prev) => ({
                                ...prev,
                                companiesRoles: [
                                  ...prev.companiesRoles,
                                  { companyId: company.id, roleId: firstRole.id },
                                ],
                              }));
                            }
                          }
                        }}
                      />
                      {formData.companiesRoles.some((cr) => cr.companyId === company.id) && (
                        <div className="ms-4 mt-2">
                          <CFormLabel className="small text-muted">
                            {t('users.selectRole') || 'Select Role:'}
                          </CFormLabel>
                          <select
                            className="form-select form-select-sm"
                            value={
                              formData.companiesRoles.find((cr) => cr.companyId === company.id)
                                ?.roleId || ''
                            }
                            onChange={(e) => {
                              const newRoleId = e.target.value;
                              setFormData((prev) => ({
                                ...prev,
                                companiesRoles: prev.companiesRoles.map((cr) =>
                                  cr.companyId === company.id
                                    ? { ...cr, roleId: newRoleId }
                                    : cr
                                ),
                              }));
                            }}
                          >
                            {roles.map((role) => (
                              <option key={role.id} value={role.id}>
                                {role.name}
                              </option>
                            ))}
                          </select>
                        </div>
                      )}
                    </CCard>
                  ))
                )}
              </div>
            </CCol>
          </CRow>

          <CModalFooter>
            <CButton color="secondary" onClick={onClose} disabled={loading}>
              {t('common.cancel') || 'Cancel'}
            </CButton>
            <CButton color="primary" type="submit" disabled={loading}>
              {loading ? <CSpinner size="sm" /> : t('common.save') || 'Save'}
            </CButton>
          </CModalFooter>
        </CForm>
      </CModalBody>
    </CModal>
  );
};

export default UserModal;
