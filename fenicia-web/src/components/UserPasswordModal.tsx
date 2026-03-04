import React, { useState } from 'react';
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
  CSpinner,
  CAlert,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilLockLocked } from '@coreui/icons';
import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const UserPasswordModal = ({ visible, onClose, onSave, userId }) => {
  const { t } = useTranslation();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [formData, setFormData] = useState({
    newPassword: '',
    confirmPassword: '',
  });

  const getToken = () => localStorage.getItem('token') || '';
  const getCompanyId = () => localStorage.getItem('companyId') || '';

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    // Validate passwords match
    if (formData.newPassword !== formData.confirmPassword) {
      setError(t('users.errors.passwordsMatch') || 'Passwords do not match');
      setLoading(false);
      return;
    }

    // Validate password length
    if (formData.newPassword.length < 6) {
      setError(t('users.errors.passwordLength') || 'Password must be at least 6 characters');
      setLoading(false);
      return;
    }

    try {
      const token = getToken();
      await axios.patch(
        `${API_BASE_URL}/user/${userId}/password`,
        { newPassword: formData.newPassword },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            'x-company': getCompanyId(),
          },
        }
      );

      onSave();
      setFormData({ newPassword: '', confirmPassword: '' });
    } catch (err) {
      setError(err.response?.data?.message || t('users.errors.passwordChangeError') || 'Error changing password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <CModal visible={visible} onClose={onClose}>
      <CModalHeader>
        <CModalTitle>
          <CIcon icon={cilLockLocked} className="me-2" />
          {t('users.changePasswordTitle') || 'Change Password'}
        </CModalTitle>
      </CModalHeader>
      <CModalBody>
        {error && (
          <CAlert color="danger" dismissible onClose={() => setError(null)}>
            {error}
          </CAlert>
        )}

        <CForm onSubmit={handleSubmit}>
          <div className="mb-3">
            <CFormLabel htmlFor="newPassword">
              {t('users.newPassword') || 'New Password'}
            </CFormLabel>
            <CFormInput
              id="newPassword"
              name="newPassword"
              type="password"
              value={formData.newPassword}
              onChange={handleInputChange}
              required
              minLength={6}
            />
          </div>

          <div className="mb-3">
            <CFormLabel htmlFor="confirmPassword">
              {t('users.confirmPassword') || 'Confirm Password'}
            </CFormLabel>
            <CFormInput
              id="confirmPassword"
              name="confirmPassword"
              type="password"
              value={formData.confirmPassword}
              onChange={handleInputChange}
              required
              minLength={6}
            />
          </div>

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

export default UserPasswordModal;
