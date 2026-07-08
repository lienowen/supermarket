using UnityEngine;

public enum EmployeeType
{
    Restocker,
    Cashier,
    Cleaner
}

[System.Serializable]
public class Employee
{
    public string name;
    public EmployeeType type;
    public int efficiency;
    public int salary;
}

public class EmployeeSystem : MonoBehaviour
{
    public int employeeCount;

    public Employee Hire(EmployeeType type)
    {
        employeeCount++;

        return new Employee()
        {
            name = "Employee_" + employeeCount,
            type = type,
            efficiency = 10,
            salary = 50
        };
    }
}
