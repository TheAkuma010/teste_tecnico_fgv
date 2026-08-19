type PageHeaderProps = {
  title: string;
  action?: React.ReactNode;
};

export function PageHeader({ title, action }: PageHeaderProps) {
  return (
    <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
      <h1 className="text-2xl font-bold text-ink">{title}</h1>
      {action}
    </div>
  );
}
