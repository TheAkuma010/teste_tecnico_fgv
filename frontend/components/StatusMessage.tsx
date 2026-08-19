type StatusMessageProps = {
  type: "success" | "error";
  message: string | null;
};

export function StatusMessage({ type, message }: StatusMessageProps) {
  if (!message) {
    return null;
  }

  const className =
    type === "success"
      ? "border-teal-200 bg-teal-50 text-teal-900"
      : "border-red-200 bg-red-50 text-red-900";

  return (
    <div className={`rounded-md border px-3 py-2 text-sm ${className}`}>
      {message}
    </div>
  );
}
