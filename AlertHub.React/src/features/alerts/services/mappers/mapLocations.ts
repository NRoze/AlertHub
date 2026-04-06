import type { Location } from "../../model/Location";
import { locationMap } from "../constants/locationMap";

export function mapLocations(names: string[]): Location[] {
  const uniqueNames = Array.from(new Set(names));
  const result: Location[] = [];

  for (const name of uniqueNames) {
    const loc = locationMap.get(name);
    if (loc) {
      result.push(loc);
    } else {
      console.warn("Unknown location:", name);
    }
  }

  return result;
}
