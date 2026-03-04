import React from 'react';
import { CPagination, CPaginationItem, CFormSelect } from '@coreui/react';

const PAGE_SIZE_OPTIONS = [2, 10, 20, 50, 100];

const Pagination = ({ pagination, onPageChange, onPerPageChange, showPageSize = true }) => {
    const { page, perPage, total, pages } = pagination;

    const totalPages = pages || Math.ceil(total / perPage) || 1;

    const handlePrevious = () => {
        if (page > 1) {
            onPageChange(page - 1);
        }
    };

    const handleNext = () => {
        if (page < totalPages) {
            onPageChange(page + 1);
        }
    };

    const renderPageNumbers = () => {
        const items = [];
        const maxVisible = 5;
        let startPage = Math.max(1, page - Math.floor(maxVisible / 2));
        let endPage = Math.min(totalPages, startPage + maxVisible - 1);

        if (endPage - startPage < maxVisible - 1) {
            startPage = Math.max(1, endPage - maxVisible + 1);
        }

        for (let i = startPage; i <= endPage; i++) {
            items.push(
                <CPaginationItem
                    key={i}
                    active={i === page}
                    onClick={() => onPageChange(i)}
                >
                    {i}
                </CPaginationItem>
            );
        }

        return items;
    };

    return (
        <div className="d-flex justify-content-between align-items-center mt-3">
            <div className="d-flex align-items-center">
                <span className="text-muted me-2">
                    {total > 0 
                        ? `Mostrando ${((page - 1) * perPage) + 1}-${Math.min(page * perPage, total)} de ${total}`
                        : 'Nenhum registro encontrado'
                    }
                </span>
                {showPageSize && (
                    <CFormSelect
                        size="sm"
                        value={perPage}
                        onChange={(e) => onPerPageChange(parseInt(e.target.value, 10))}
                        style={{ width: 'auto' }}
                        aria-label="Seleccionar tamanho da página"
                    >
                        {PAGE_SIZE_OPTIONS.map(size => (
                            <option key={size} value={size}>
                                {size} por página
                            </option>
                        ))}
                    </CFormSelect>
                )}
            </div>
            <CPagination>
                <CPaginationItem
                    onClick={handlePrevious}
                >
                    Anterior
                </CPaginationItem>
                {renderPageNumbers()}
                <CPaginationItem
                    onClick={handleNext}
                >
                    Próximo
                </CPaginationItem>
            </CPagination>
        </div>
    );
};

export default Pagination;
