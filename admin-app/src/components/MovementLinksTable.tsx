import React from 'react';
import { useTheme } from '../theme/useTheme';
import { MbdMovementLink } from '../types';
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
  link: MbdMovementLink;
  onEdit: (link: MbdMovementLink) => void;
  onDelete: (id: string) => void;
  index: number;
}

const SortableRow: React.FC<SortableRowProps> = ({ link, onEdit, onDelete, index }) => {
  const { colors } = useTheme();
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: link.id || '' });

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
          onClick={() => onEdit(link)}
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
          onClick={() => onDelete(link.id!)}
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
      <td style={{ padding: '16px', fontWeight: '500', color: colors.foreground }}>{link.title}</td>
      <td style={{ padding: '16px', color: colors.mutedText }}>
        <a href={link.url} target="_blank" rel="noopener noreferrer" style={{ color: colors.primary, textDecoration: 'none' }}>
          {link.url}
        </a>
      </td>
      <td style={{ padding: '16px', color: colors.mutedText }}>{link.order}</td>
    </tr>
  );
};

interface MovementLinksTableProps {
  links: MbdMovementLink[];
  onEdit: (link: MbdMovementLink) => void;
  onDelete: (id: string) => void;
  onReorder: (newLinks: MbdMovementLink[]) => void;
}

const MovementLinksTable: React.FC<MovementLinksTableProps> = ({ links, onEdit, onDelete, onReorder }) => {
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
      const oldIndex = links.findIndex((l) => l.id === active.id);
      const newIndex = links.findIndex((l) => l.id === over?.id);

      const reordered = arrayMove(links, oldIndex, newIndex);
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
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Title</th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>URL</th>
                <th style={{ padding: '16px', textAlign: 'left', fontWeight: '600', color: colors.mutedText }}>Order</th>
              </tr>
            </thead>
            <tbody>
              <SortableContext
                items={links.map(l => l.id || '')}
                strategy={verticalListSortingStrategy}
              >
                {links.map((link, index) => (
                  <SortableRow
                    key={link.id}
                    link={link}
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
      {links.length === 0 && (
        <div style={{ padding: '40px 20px', textAlign: 'center', color: colors.placeholder }}>
          No Movement Links found
        </div>
      )}
    </div>
  );
};

export default MovementLinksTable;