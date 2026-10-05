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
        
        const ad = await Api.getSparePart(id);
        
        if (ad && (ad.sellerId === user.id || ad.sellerUsername === user.username)) {
          // Change to actual warehouse ad route if different
          navigate(`/warehouse/ads/${id}`, { replace: true });
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
