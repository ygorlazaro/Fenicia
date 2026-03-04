import React from 'react';
import { useTranslation } from 'react-i18next';
import { CPagination, CFormSelect } from '@coreui/react';

const PAGE_SIZE_OPTIONS = [2, 10, 20, 50, 100];

const Pagination = ({ pagination, onPageChange, onPerPageChange, showPageSize = true }) => {
    const { t } = useTranslation();
    const { page, perPage, total, pages } = pagination;

    const totalPages = Math.ceil(total / perPage) || 1;
    const hasNextPage = page < totalPages;
    const hasPrevPage = page > 1;

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
                <li 
                    key={i} 
                    className={`page-item ${i === page ? 'active' : ''}`}
                    style={{ cursor: 'pointer' }}
                    onClick={() => onPageChange(i)}
                >
                    <span className="page-link">{i}</span>
                </li>
            );
        }

        return items;
    };

    return (
        <div className="d-flex justify-content-between align-items-center mt-3">
            <div className="d-flex align-items-center">
                <span className="text-muted me-2">
                    {total > 0 
                        ? t('common.showing', { start: ((page - 1) * perPage) + 1, end: Math.min(page * perPage, total), total })
                        : t('common.noData')
                    }
                </span>
                {showPageSize && (
                    <CFormSelect
                        size="sm"
                        value={perPage}
                        onChange={(e) => onPerPageChange(parseInt(e.target.value, 10))}
                        style={{ width: 'auto' }}
                        aria-label={t('common.selectPageSize')}
                    >
                        {PAGE_SIZE_OPTIONS.map(size => (
                            <option key={size} value={size}>
                                {size} {t('common.perPage')}
                            </option>
                        ))}
                    </CFormSelect>
                )}
            </div>
            <CPagination>
                <li 
                    className={`page-item ${!hasPrevPage ? 'disabled' : ''}`}
                    style={{ 
                        pointerEvents: hasPrevPage ? 'auto' : 'none',
                        opacity: hasPrevPage ? 1 : 0.5
                    }}
                    onClick={(e) => {
                        e.preventDefault();
                        if (hasPrevPage) {
                            console.log('Clicked Previous, going to page:', page - 1);
                            onPageChange(page - 1);
                        }
                    }}
                >
                    <a className="page-link" href="#" style={{ pointerEvents: 'none' }}>{t('common.previous')}</a>
                </li>
                {renderPageNumbers()}
                <li 
                    className={`page-item ${!hasNextPage ? 'disabled' : ''}`}
                    style={{ 
                        pointerEvents: hasNextPage ? 'auto' : 'none',
                        opacity: hasNextPage ? 1 : 0.5
                    }}
                    onClick={(e) => {
                        e.preventDefault();
                        if (hasNextPage) {
                            console.log('Clicked Next, going to page:', page + 1);
                            onPageChange(page + 1);
                        }
                    }}
                >
                    <a className="page-link" href="#" style={{ pointerEvents: 'none' }}>{t('common.next')}</a>
                </li>
            </CPagination>
        </div>
    );
};

export default Pagination;
