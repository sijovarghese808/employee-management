namespace employee.api.DTOs
{
    public class DesignationDTO<T>
    {

        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
}

    public class DesignationSuccessDto
    {
        public int designationId { get; set; }
        public int departmentId { get; set; }
        public string designationName { get; set; }
    }
}
