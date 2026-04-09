import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import type { ActiveAlertLocation } from '../../shared/model/ActiveAlertLocation'

interface LocationState {
  selectedLocations: ActiveAlertLocation[];
  addLocation: (loc: ActiveAlertLocation) => void;
  removeLocation: (id: string) => void;
}

export const useLocationStore = create<LocationState>()(
  persist(
    (set) => ({
      selectedLocations: [],
      addLocation: (loc) => set((state) => {
        if (state.selectedLocations.some(l => l.id === loc.id)) return state;
        return { selectedLocations: [...state.selectedLocations, loc] };
      }),
      removeLocation: (id) => set((state) => ({
        selectedLocations: state.selectedLocations.filter(l => l.id !== id)
      })),
    }),
    {
      name: 'monitored_locations_storage', // Unique Key
      storage: createJSONStorage(() => localStorage),
    }
  )
);