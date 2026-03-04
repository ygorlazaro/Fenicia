import React, { useEffect, useState } from 'react';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CFormTextarea,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert
} from '@coreui/react';

const ProjectCommentModal = ({
    visible,
    onClose,
    onSave,
    projectComment,
    loading
}) => {
    const [formData, setFormData] = useState({
        content: '',
        taskId: '',
        userId: ''
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (projectComment) {
            setFormData({
                content: projectComment.content || '',
                taskId: projectComment.taskId || '',
                userId: projectComment.userId || ''
            });
        } else {
            setFormData({
                content: '',
                taskId: '',
                userId: ''
            });
        }
        setError(null);
    }, [projectComment, visible]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        setError(null);

        // Validation
        if (!formData.content) {
            setError('Conteúdo é obrigatório.');
            return;
        }

        onSave(formData);
    };

    return (
        <CModal
            visible={visible}
            onClose={onClose}
            size="lg"
        >
            <CModalHeader>
                <CModalTitle>
                    {projectComment ? 'Editar Comentário do Projeto' : 'Novo Comentário do Projeto'}
                </CModalTitle>
            </CModalHeader>
            <CForm onSubmit={handleSubmit}>
                <CModalBody>
                    {error && (
                        <CAlert color="danger" dismissible>
                            {error}
                        </CAlert>
                    )}

                    <div className="mb-3">
                        <CFormLabel htmlFor="content">Conteúdo *</CFormLabel>
                        <CFormTextarea
                            id="content"
                            name="content"
                            value={formData.content}
                            onChange={handleInputChange}
                            rows={5}
                            required
                        />
                    </div>

                    <div className="row">
                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="taskId">ID da Tarefa</CFormLabel>
                            <CFormInput
                                type="text"
                                id="taskId"
                                name="taskId"
                                value={formData.taskId}
                                onChange={handleInputChange}
                            />
                        </div>

                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="userId">ID do Usuário</CFormLabel>
                            <CFormInput
                                type="text"
                                id="userId"
                                name="userId"
                                value={formData.userId}
                                onChange={handleInputChange}
                            />
                        </div>
                    </div>
                </CModalBody>
                <CModalFooter>
                    <CButton color="secondary" onClick={onClose} disabled={loading}>
                        Cancelar
                    </CButton>
                    <CButton
                        color="primary"
                        type="submit"
                        disabled={loading}
                    >
                        {loading ? 'Salvando...' : 'Salvar'}
                    </CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
};

export default ProjectCommentModal;
