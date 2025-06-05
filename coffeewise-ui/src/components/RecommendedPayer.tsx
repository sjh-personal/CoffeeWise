import { Alert, Box, Skeleton } from "@mui/material";
import type { PersonDto } from "../types/dto";

const ALERT_HEIGHT = 56;

export default function RecommendedPayer({
  recommended,
  loading,
}: {
  recommended: PersonDto | null;
  loading: boolean;
}) {
  return (
    <Box
      sx={{
        minHeight: ALERT_HEIGHT,
        my: 2,
        display: "flex",
        alignItems: "center",
      }}
    >
      {loading ? (
        <Skeleton variant="rounded" height={40} width="100%" />
      ) : !recommended ? (
        <Alert severity="warning" sx={{ width: "100%" }}>
          No recommended payer (nobody is present).
        </Alert>
      ) : (
        <Alert severity="info" sx={{ width: "100%" }}>
          <strong>Recommended to pay today:</strong> {recommended.name} (
          {recommended.email})
        </Alert>
      )}
    </Box>
  );
}
