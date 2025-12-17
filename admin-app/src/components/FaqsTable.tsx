import React from 'react';
import { useTheme } from '../theme/useTheme';
import { Faq } from '../types';
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragEndEvent,
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
  useSortable,
} from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { GripVertical } from 'lucide-react';

interface SortableRowProps {
  faq: Faq;
  onEdit: (faq: Faq) => void;
  onDelete: (id: string) => void;
  index: number;
}

const SortableRow: React.FC<SortableRowProps> = ({ faq, onEdit, onDelete, index }) => {
  const { colors } = useTheme();
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: faq.id || '' });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    backgroundColor: isDragging 
      ? colors.neutral 
      : (index % 2 === 0 ? colors.background : colors.backgroundSecondary),
    borderBottom: `1px solid ${colors.border}`,
    zIndex: isDragging ? 1 : 0,
    opacity: isDragging ? 0.8 : 1,
  };

  return (
    <tr ref={setNodeRef} style={style}>
      <td style={{ padding: '16px', width: '40px' }}>
        <div 
          {...attributes} 
          {...listeners} 
          style={{ cursor: 'grab', display: 'flex', alignItems: 'center', color: colors.mutedText }}
        >
          <GripVertical size={18} />
        </div>
      </td>
      <td style={{ padding: '16px', whiteSpace: 'nowrap' }}>
        <button
          onClick={() => onEdit(faq)}
          style={{
            padding: '6px 12px',
            marginRight: '8px',
            fontSize: '12px',
            fontWeight: '600',
            color: colors.primary,
            backgroundColor: colors.primaryLight,
            border: `1px solid ${colors.primary}`,
            borderRadius: '4px',
            cursor: 'pointer',
            transition: 'all 0.2s',
          }}
          onMouseEnter={(e) => {
            e.currentTarget.style.backgroundColor = colors.primary;
            e.currentTarget.style.color = '#fff';
          }}
          onMouseLeave={(e) => {
            e.currentTarget.style.backgroundColor = colors.primaryLight;
            e.currentTarget.style.color = colors.primary;
          }}
        >
          Edit
        </button>
        <button
          onClick={() => onDelete(faq.id!)}
          style={{
            padding: '6px 12px',
            fontSize: '12px',
            fontWeight: '600',
            color: colors.danger,
            backgroundColor: colors.dangerLight,
            border: `1px solid ${colors.danger}`,
            borderRadius: '4px',
            cursor: 'pointer',
            transition: 'all 0.2s',
          }}
          onMouseEnter={(e) => {
            e.currentTarget.style.backgroundColor = colors.danger;
            e.currentTarget.style.color = '#fff';
          }}
          onMouseLeave={(e) => {
            e.currentTarget.style.backgroundColor = colors.dangerLight;
            e.currentTarget.style.color = colors.danger;
          }}
        >
          Delete
        </button>
      </td>
      <td style={{ padding: '16px', fontWeight: '500', color: colors.foreground }}>{faq.question}</td>
      <td style={{ padding: '16px', color: colors.mutedText, maxWidth: '200px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
        {faq.shortAnswer}
      </td>
      <td style={{ padding: '16px', color: colors.mutedText, maxWidth: '400px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
        {faq.answer}
      </td>
      <td style={{ padding: '16px', color: colors.mutedText }}>{faq.order}</td>
    </tr>
  );
};

interface FaqsTableProps {
  faqs: Faq[];
  onEdit: (faq: Faq) => void;
  onDelete: (id: string) => void;
  onReorder: (newFaqs: Faq[]) => void;
}

const FaqsTable: React.FC<FaqsTableProps> = ({ faqs, onEdit, onDelete, onReorder }) => {
  const { colors } = useTheme();

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (active.id !== over?.id) {
      const oldIndex = faqs.findIndex((f) => f.id === active.id);
      const newIndex = faqs.findIndex((f) => f.id === over?.id);

      const reordered = arrayMove(faqs, oldIndex, newIndex);
      onReorder(reordered);
    }
  };

  return (
    <div style={{ backgroundColor: colors.background, borderRadius: '8px', border: `1px solid ${colors.border}`, overflow: 'hidden', marginBottom: '20px' }}>
      <div style={{ overflowX: 'auto' }}>
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragEnd={handleDragEnd}
        >
          <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
            <thead>
              <tr style={{ backgroundColor: colors.backgroundSecondary, borderBottom: `2px solid ${colors.border}` }}>
                <th style={{ padding: '16px', width: '40px' }}></th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Actions</th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Question</th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Short Answer</th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Answer</th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Order</th>
              </tr>
            </thead>
            <tbody>
              <SortableContext
                items={faqs.map(f => f.id || '')}
                strategy={verticalListSortingStrategy}
              >
                {faqs.map((faq, index) => (
                  <SortableRow
                    key={faq.id}
                    faq={faq}
                    onEdit={onEdit}
                    onDelete={onDelete}
                    index={index}
                  />
                ))}
              </SortableContext>
            </tbody>
          </table>
        </DndContext>
      </div>
      {faqs.length === 0 && (
        <div style={{ padding: '40px 20px', textAlign: 'center', color: colors.placeholder }}>
          No FAQs found
        </div>
      )}
    </div>
  );
};

export default FaqsTable;