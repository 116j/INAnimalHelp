package crc6424e3323fbe89298f;


public class FacebookAuth
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("INAnimalHelp.Droid.FacebookAuth, INAnimalHelp.Android", FacebookAuth.class, __md_methods);
	}


	public FacebookAuth ()
	{
		super ();
		if (getClass () == FacebookAuth.class)
			mono.android.TypeManager.Activate ("INAnimalHelp.Droid.FacebookAuth, INAnimalHelp.Android", "", this, new java.lang.Object[] {  });
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
