package md530e360d6e887751630c8cb8d7320c899;


public class MemoryController
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Bt2Arduino.MemoryController, Bt2Arduino", MemoryController.class, __md_methods);
	}


	public MemoryController ()
	{
		super ();
		if (getClass () == MemoryController.class)
			mono.android.TypeManager.Activate ("Bt2Arduino.MemoryController, Bt2Arduino", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
