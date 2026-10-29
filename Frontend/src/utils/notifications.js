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
  try {
    const list = getNotifications(user.id);
    const filtered = list.filter(n => n.title !== 'Xoş gəldiniz!' && n.title !== 'Kataloq və Elan İdarəetməsi' && n.title !== 'Platformaya xoş gəldiniz!');
    if (filtered.length !== list.length) {
      saveNotifications(user.id, filtered);
    }
  } catch {}
}
