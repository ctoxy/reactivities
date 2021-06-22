//permet de retourner le resultat dans l'activité pour eviter les exeption ex activité inexistante 
namespace Application.Core
{
    //t est un terme générique qui sera remplacer ici le cas échéant par activity 
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public string Error { get; set; }

        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
        public static Result<T> Failure(string error) => new Result<T> { IsSuccess = false, Error = error };
    }
}