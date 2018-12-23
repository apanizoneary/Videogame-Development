public class Tuple<T1, T2>
{
	public T1 First { get; set; }
	public T2 Second { get; set; }
	internal Tuple(T1 first, T2 second)
	{
		First = first;
		Second = second;
	}
}

//public static class tuple
//{
//    public static tuple<t1, t2> new<t1, t2>(t1 first, t2 second)
//    {
//        var tuple = new tuple<t1, t2>(first, second);
//        return tuple;
//    }
//}