import { CFormInput, CFormLabel } from "@coreui/react";
import { useTranslation } from "react-i18next";

interface FeniciaInputProps {
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  onBlur?: (e: React.FocusEvent<HTMLInputElement>) => void;
  value: string | number;
  label: string;
  id: string;
  required?: boolean;
  placeholder?: string;
  type?: 'text' | 'email' | 'password' | 'number' | 'tel' | 'url' | 'date' | 'datetime-local' | 'month' | 'week' | 'time';
  maxLength?: number;
  min?: number;
  step?: number;
}

export function FeniciaInput({
  onChange,
  value,
  label,
  id,
  required,
  type = 'text',
  onBlur,
  placeholder,
  maxLength,
  min = null,
  step = null
}: FeniciaInputProps) {
  const { t } = useTranslation();

  return <>
    <CFormLabel htmlFor={id}>{t(label)} {required && <span className="text-danger">*</span>}</CFormLabel>
    <CFormInput type={type} id={id} name={id} value={value} onChange={onChange} onBlur={onBlur} required={required} placeholder={placeholder} maxLength={maxLength} min={min} step={step} />
  </>;
}
