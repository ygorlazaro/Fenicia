import React, { useEffect, useState } from 'react';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert
} from '@coreui/react';

const ProjectAttachmentModal = ({
    visible,
    onClose,
    onSave,
    projectAttachment,
    loading
}) => {
    const [formData, setFormData] = useState({
        fileName: '',
        fileUrl: '',
        fileSize: '',
        uploadedBy: '',
        taskId: ''
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (projectAttachment) {
            setFormData({
                fileName: projectAttachment.fileName || '',
                fileUrl: projectAttachment.fileUrl || '',
                fileSize: projectAttachment.fileSize?.toString() || '',
                uploadedBy: projectAttachment.uploadedBy || '',
                taskId: projectAttachment.taskId || ''
            });
        } else {
            setFormData({
                fileName: '',
                fileUrl: '',
                fileSize: '',
                uploadedBy: '',
                taskId: ''
            });
        }
        setError(null);
    }, [projectAttachment, visible]);

    const handleInputChange = (e) => {
        const { name, value, type } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'number' ? (value ? parseInt(value, 10) : '') : value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        setError(null);

        // Validation
        if (!formData.fileName) {
            setError('Nome do arquivo é obrigatório.');
            return;
        }

        // Prepare data for submission
        const submitData = {
            ...formData,
            fileSize: formData.fileSize ? parseInt(formData.fileSize, 10) : null
        };

        onSave(submitData);
    };

    return (
        <CModal
            visible={visible}
            onClose={onClose}
            size="lg"
        >
            <CModalHeader>
                <CModalTitle>
                    {projectAttachment ? 'Editar Anexo do Projeto' : 'Novo Anexo do Projeto'}
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
                        <CFormLabel htmlFor="fileName">Nome do Arquivo *</CFormLabel>
                        <CFormInput
                            type="text"
                            id="fileName"
                            name="fileName"
                            value={formData.fileName}
                            onChange={handleInputChange}
                            required
                        />
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="fileUrl">URL do Arquivo</CFormLabel>
                        <CFormInput
                            type="url"
                            id="fileUrl"
                            name="fileUrl"
                            value={formData.fileUrl}
                            onChange={handleInputChange}
                            placeholder="https://exemplo.com/arquivo.pdf"
                        />
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="fileSize">Tamanho do Arquivo (bytes)</CFormLabel>
                        <CFormInput
                            type="number"
                            id="fileSize"
                            name="fileSize"
                            value={formData.fileSize}
                            onChange={handleInputChange}
                            min="0"
                            placeholder="Ex: 1048576 (1MB)"
                        />
                    </div>

                    <div className="row">
                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="uploadedBy">Enviado Por</CFormLabel>
                            <CFormInput
                                type="text"
                                id="uploadedBy"
                                name="uploadedBy"
                                value={formData.uploadedBy}
                                onChange={handleInputChange}
                            />
                        </div>

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

export default ProjectAttachmentModal;
