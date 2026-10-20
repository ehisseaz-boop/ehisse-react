import Swal from 'sweetalert2';

/**
 * Custom confirmation dialog styled to match the site's dark/light design.
 * Replaces native browser window.confirm(...) popups.
 * 
 * @param {string|object} optionsOrMessage - Text prompt or config object
 * @returns {Promise<boolean>} True if confirmed, false otherwise
 */
export const confirmModal = async (optionsOrMessage) => {
  let title = 'Təsdiqləmə';
  let text = '';
  let confirmText = 'Təsdiq edirəm';
  let cancelButtonText = 'Ləğv et';
  let isDanger = false;
  let icon = 'question';

  if (typeof optionsOrMessage === 'string') {
    text = optionsOrMessage;
    const lower = text.toLowerCase();
    if (lower.includes('sil') || lower.includes('blok') || lower.includes('ayır')) {
      isDanger = true;
      icon = 'warning';
      confirmText = lower.includes('sil') ? 'Bəli, silinsin' : 'Təsdiq edirəm';
    }
  } else if (typeof optionsOrMessage === 'object' && optionsOrMessage !== null) {
    text = optionsOrMessage.text || optionsOrMessage.message || '';
    title = optionsOrMessage.title || title;
    const lower = text.toLowerCase();
    isDanger = optionsOrMessage.isDanger ?? (lower.includes('sil') || lower.includes('blok') || lower.includes('ayır'));
    confirmText = optionsOrMessage.confirmButtonText || optionsOrMessage.confirmText || (isDanger && lower.includes('sil') ? 'Bəli, silinsin' : 'Təsdiq edirəm');
    cancelButtonText = optionsOrMessage.cancelButtonText || optionsOrMessage.cancelText || 'Ləğv et';
    icon = optionsOrMessage.icon || (isDanger ? 'warning' : 'question');
  }

  const result = await Swal.fire({
    title,
    text,
    icon,
    showCancelButton: true,
    confirmButtonText,
    cancelButtonText,
    reverseButtons: true,
    buttonsStyling: false,
    customClass: {
      popup: 'site-confirm-popup',
      title: 'site-confirm-title',
      htmlContainer: 'site-confirm-text',
      actions: 'site-confirm-actions',
      confirmButton: isDanger ? 'site-confirm-btn site-confirm-danger' : 'site-confirm-btn site-confirm-primary',
      cancelButton: 'site-confirm-btn site-confirm-cancel'
    }
  });

  return !!result.isConfirmed;
};

/**
 * Custom alert dialog styled to match site design.
 */
export const alertModal = async (message, title = 'Məlumat', icon = 'info') => {
  return await Swal.fire({
    title,
    text: message,
    icon,
    confirmButtonText: 'Bağla',
    buttonsStyling: false,
    customClass: {
      popup: 'site-confirm-popup',
      title: 'site-confirm-title',
      htmlContainer: 'site-confirm-text',
      actions: 'site-confirm-actions',
      confirmButton: 'site-confirm-btn site-confirm-primary'
    }
  });
};

/**
 * Custom prompt dialog for inputs.
 */
export const promptModal = async ({
  title = 'Məlumat daxil edin',
  inputLabel = '',
  inputValue = '',
  inputType = 'text',
  confirmText = 'Təsdiq et',
  cancelText = 'Ləğv et'
} = {}) => {
  const result = await Swal.fire({
    title,
    input: inputType,
    inputLabel,
    inputValue,
    showCancelButton: true,
    confirmButtonText: confirmText,
    cancelButtonText: cancelText,
    reverseButtons: true,
    buttonsStyling: false,
    customClass: {
      popup: 'site-confirm-popup',
      title: 'site-confirm-title',
      input: 'site-confirm-input form-control',
      actions: 'site-confirm-actions',
      confirmButton: 'site-confirm-btn site-confirm-primary',
      cancelButton: 'site-confirm-btn site-confirm-cancel'
    }
  });

  if (result.isConfirmed) {
    return result.value;
  }
  return null;
};

export default confirmModal;
