import {
  CAlert,
  CCol,
  CContainer,
  CRow,
  CSpinner
} from '@coreui/react';
import { useEffect, useState } from 'react';
import AuthProfileClient from '../../services/auth/auth-profile-client';
import { GetUserProfileResponse } from '../../types/auth-types';
import UserCompanies from './user-companies';
import UserProfile from './user-profile';
import UserSubscriptionsDetail from './user-subscriptions-detail';
import UserSubscriptionsSummary from './user-subscriptions-summary';

const profileClient = new AuthProfileClient();

const Profile = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [profile, setProfile] = useState<GetUserProfileResponse | null>(null);

  useEffect(() => {
    loadProfile();
  }, []);

  const loadProfile = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await profileClient.getProfile();
      setProfile(data);
    } catch (err) {
      console.error('Failed to load profile:', err);
      setError(err.response?.data?.title || 'Falha ao carregar perfil.');
    } finally {
      setLoading(false);
    }
  };



  if (loading) {
    return (
      <CContainer className="py-4">
        <div className="text-center py-5">
          <CSpinner color="primary" />
          <p className="mt-3">Carregando perfil...</p>
        </div>
      </CContainer>
    );
  }

  if (error) {
    return (
      <CContainer className="py-4">
        <CAlert color="danger">{error}</CAlert>
      </CContainer>
    );
  }

  if (!profile) {
    return (
      <CContainer className="py-4">
        <CAlert color="warning">Perfil não encontrado.</CAlert>
      </CContainer>
    );
  }

  return (
    <CContainer className="py-4">
      <CRow>
        <CCol md={8}>
          <UserProfile profile={profile} />
          <UserCompanies companies={profile.companies} />
        </CCol>

        <UserSubscriptionsSummary subscriptions={profile.subscriptions} />
      </CRow>

      <UserSubscriptionsDetail subscriptions={profile.subscriptions} />

    </CContainer>
  );
};

export default Profile;
