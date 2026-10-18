import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Api } from '../api';

function ElanRedirect({ user }) {
  const { id } = useParams();
  const navigate = useNavigate();

  useEffect(() => {
    const checkOwnership = async () => {
      try {
        if (!user) {
          navigate(`/details/${id}`, { replace: true });
          return;
        }
        
        const res = await Api.details(id);
        const part = res?.part;
        const seller = res?.seller;
        
        // Elanın cari istifadəçiyə aid olmasını yoxla
        if (user && (seller?.userId === user.id || part?.sellerId === user.sellerId || part?.sellerUserId === user.id)) {
          navigate(`/warehouse/part/${id}`, { replace: true });
        } else {
          navigate(`/details/${id}`, { replace: true });
        }
      } catch (err) {
        navigate(`/details/${id}`, { replace: true });
      }
    };
    
    checkOwnership();
  }, [id, user, navigate]);

  return (
    <div className="container text-center mt-5">
      <h4>Yönləndirilir...</h4>
    </div>
  );
}

export default ElanRedirect;
