import React from 'react';
import { Link } from 'react-router-dom';

function NotFound() {
  return (
    <div className="container text-center py-5">
      <h2 className="section-title">404 — Səhifə tapılmadı</h2>
      <Link to="/" className="link-accent">Ana səhifəyə qayıt</Link>
    </div>
  );
}

export default NotFound;
