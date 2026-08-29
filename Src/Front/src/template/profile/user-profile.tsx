// @ts-nocheck
import CIcon from "@coreui/icons-react";
import { cilUser } from "@coreui/icons/dist/esm/free/cil-user";
import { CCard, CCardBody, CCardHeader, CCol, CRow } from "@coreui/react";
import { GetUserProfileResponse } from "../../types/auth/get-user-profile-response";

interface UserProfileProps {
    profile: GetUserProfileResponse;
}

const UserProfile = ({ profile }: UserProfileProps) => {
    return (
        <CCard className="mb-4">
            <CCardHeader>
                <strong>
                    <CIcon icon={cilUser} className="me-2" />
                    Informações do Usuário
                </strong>
            </CCardHeader>
            <CCardBody>
                <CRow>
                    <CCol md={6}>
                        <h6 className="text-muted">Nome</h6>
                        <p className="fs-5">{profile.name}</p>
                    </CCol>
                    <CCol md={6}>
                        <h6 className="text-muted">E-mail</h6>
                        <p className="fs-5">
                            <a href={`mailto:${profile.email}`} className="text-decoration-none">
                                {profile.email}
                            </a>
                        </p>
                    </CCol>
                </CRow>
            </CCardBody>
        </CCard>
    );
};

export default UserProfile;
