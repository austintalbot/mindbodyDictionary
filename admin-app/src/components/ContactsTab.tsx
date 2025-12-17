// admin-app/src/components/ContactsTab.tsx
import React, { useEffect, useState } from 'react';
import { fetchContactsTable, deleteContact } from '../services/apiService';

interface Contact {
  id: string;
  email: string;
  saveDateTime: string; // Assuming it's a string, can be converted to Date object for display
}

const ContactsTab: React.FC = () => {
  const [contacts, setContacts] = useState<Contact[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [alertMessage, setAlertMessage] = useState<string | null>(null);

  useEffect(() => {
    loadContacts();
  }, []);

  const loadContacts = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await fetchContactsTable();
      setContacts(data);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch contacts');
    } finally {
      setLoading(false);
    }
  };

  const deleteContactConfirm = async (id: string, email: string) => {
    if (window.confirm(`Are you sure you want to delete ${email}?`)) {
      try {
        await deleteContact(id, email);
        setAlertMessage('Contact deleted successfully!');
        loadContacts(); // Reload table after deletion
      } catch (err: any) {
        setError(err.message || 'Failed to delete contact');
      }
    }
  };

  // The original HTML used DataTables with 'excel' button.
  // This likely relies on a DataTables extension that generates the Excel file client-side or
  // triggers a backend endpoint. For simplicity, we'll assume the backend provides an endpoint
  // if this is a direct export, or integrate a client-side library if necessary.
  // For now, this will be a placeholder.
  const exportToExcel = () => {
    setAlertMessage('Export to Excel functionality coming soon!');
    // Implement actual export logic here, potentially calling a backend endpoint
    // or using a client-side library to generate the CSV/Excel.
  };


  if (loading) return <div>Loading Contacts...</div>;
  if (error) return <div className="alert alert-danger">Error: {error}</div>;

  return (
    <div className="tab-pane fade show active" id="nav-contact" role="tabpanel" aria-labelledby="nav-contact-tab">
      <div className="card">
        <div className="card-body">
          <h5 className="card-title">Contacts</h5>
          <button className="btn btn-primary" onClick={loadContacts}>Refresh Contacts</button>
          <div id="contactsInternalDiv" className="mt-3">
            <table className="display" style={{ width: '100%' }}>
              <thead>
                <tr>
                  <th>Email</th>
                  <th>Save DateTime</th>
                  <th>Delete</th>
                </tr>
              </thead>
              <tbody>
                {contacts.map((contact) => (
                  <tr key={contact.id}>
                    <td>{contact.email}</td>
                    <td>{new Date(contact.saveDateTime).toLocaleString()}</td>
                    <td>
                      <button className="btn btn-outline-dark" onClick={() => deleteContactConfirm(contact.id, contact.email)}>
                        <i className="fas fa-trash"></i>
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            <button type="button" className="btn btn-sm btn-outline-primary mt-3" onClick={exportToExcel}>
                Export to Excel
            </button>
          </div>
          {alertMessage && (
            <div className="alert alert-info mt-3" role="alert">
              {alertMessage}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ContactsTab;
