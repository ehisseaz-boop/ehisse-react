const STORAGE_KEY = 'ehisse_notifications';

/**
 * Get all notifications for current user from localStorage
 */
export function getNotifications(userId) {
  try {
    const raw = localStorage.getItem(`${STORAGE_KEY}_${userId || 'anon'}`);
    return raw ? JSON.parse(raw) : [];
  } catch {
    return [];
  }
}

/**
 * Save notifications list
 */
function saveNotifications(userId, list) {
  try {
    localStorage.setItem(`${STORAGE_KEY}_${userId || 'anon'}`, JSON.stringify(list));
  } catch (e) {
    console.error('Bildirişləri saxlamaq mümkün olmadı:', e);
  }
}

/**
 * Add a new notification
 */
export function addNotification(userId, { title, message, type = 'info', link = null, icon = 'bi-bell' }) {
  const list = getNotifications(userId);
  const newItem = {
    id: Date.now() + Math.random().toString(36).substring(2, 6),
    title,
    message,
    type, // 'success', 'warning', 'info', 'danger'
    link,
    icon,
    createdAt: new Date().toISOString(),
    isRead: false
  };

  list.unshift(newItem);
  // Max 50 notifications keep
  if (list.length > 50) list.pop();
  saveNotifications(userId, list);

  // Dispatch custom event for real-time UI updates
  window.dispatchEvent(new CustomEvent('ehisse_notification_change', { detail: { userId } }));
  return newItem;
}

/**
 * Mark a single notification as read
 */
export function markAsRead(userId, notificationId) {
  const list = getNotifications(userId);
  const item = list.find(n => n.id === notificationId);
  if (item) {
    item.isRead = true;
    saveNotifications(userId, list);
    window.dispatchEvent(new CustomEvent('ehisse_notification_change', { detail: { userId } }));
  }
}

/**
 * Mark all notifications as read
 */
export function markAllAsRead(userId) {
  const list = getNotifications(userId);
  list.forEach(n => { n.isRead = true; });
  saveNotifications(userId, list);
  window.dispatchEvent(new CustomEvent('ehisse_notification_change', { detail: { userId } }));
}

/**
 * Clear all notifications
 */
export function clearNotifications(userId) {
  saveNotifications(userId, []);
  window.dispatchEvent(new CustomEvent('ehisse_notification_change', { detail: { userId } }));
}

/**
 * Initialize default welcome/system notifications if empty
 */
export function initDefaultNotifications(user) {
  if (!user?.id) return;
  const list = getNotifications(user.id);
  if (list.length === 0) {
    if (user.role === 'Anbardar' || user.role === 'Admin') {
      addNotification(user.id, {
        title: 'Xoş gəldiniz!',
        message: 'Artıq Excel faylı ilə 1 kliklə minlərlə elan yükləyə və ya mövcud elanları idarə edə bilərsiniz.',
        link: '/warehouse',
        icon: 'bi-file-earmark-excel',
        type: 'success'
      });
      addNotification(user.id, {
        title: 'Kataloq və Elan İdarəetməsi',
        message: 'Elanlarınızın statusunu "Elanlarım" bölməsindən canlı izləyə bilərsiniz.',
        link: '/warehouse',
        icon: 'bi-speedometer2',
        type: 'info'
      });
    } else {
      addNotification(user.id, {
        title: 'Platformaya xoş gəldiniz!',
        message: 'Doğru ehtiyat hissəsini axtarış və kateqoriya filtri ilə asanlıqla tapa bilərsiniz.',
        link: '/',
        icon: 'bi-check-circle',
        type: 'success'
      });
    }
  }
}
