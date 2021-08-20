namespace MyMediaCollection.Interfaces
{
    public interface IValidatable
    {
        /// <summary>
        /// Validate the value of the member name.
        /// </summary>
        /// <param name="memberName">The member name to validate.</param>
        /// <param name="value">The value to validate against the member name.</param>
        void Validate(string memberName, object value);
    }
}