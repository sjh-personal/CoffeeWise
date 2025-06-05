import {
  Box,
  Checkbox,
  FormControlLabel,
  Typography,
  Stack,
} from "@mui/material";
import type { PersonDto } from "../types/dto";

export default function PresenceForm({
  members,
  presentMap,
  onChange,
}: {
  members: PersonDto[];
  presentMap: Record<string, boolean>;
  onChange: (personId: string, isPresent: boolean) => void;
}) {
  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Today’s Coffee Crew — Who’s In?
      </Typography>
      <Stack direction="row" flexWrap="wrap" gap={2}>
        {members.map((m) => (
          <FormControlLabel
            key={m.personId}
            control={
              <Checkbox
                checked={!!presentMap[m.personId]}
                onChange={() => onChange(m.personId, !presentMap[m.personId])}
              />
            }
            label={m.name}
          />
        ))}
      </Stack>
    </Box>
  );
}
