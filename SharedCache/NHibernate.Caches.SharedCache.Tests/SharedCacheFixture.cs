#region License

//
//  SharedCache - A cache provider for NHibernate using indeXus.Net Shared Cache
//  (http://www.sharedcache.com/).
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// CLOVER:OFF
//

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using log4net.Config;
using NHibernate.Cache;
using NUnit.Framework;

namespace NHibernate.Caches.SharedCache.Tests
{
	[TestFixture]
	public class SharedCacheFixture
	{
		private SharedCacheProvider provider;
		private Dictionary<string, string> props;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			XmlConfigurator.Configure();
			props = new Dictionary<string, string>();
			provider = new SharedCacheProvider();
			provider.Start(props);
		}

		[TestFixtureTearDown]
		public void FixtureStop()
		{
			provider.Stop();
		}

		[Test]
		public void TestClear()
		{
			string key = "key1";
			string value = "value";

			ICache cache = provider.BuildCache("nunit", props);
			Assert.IsNotNull(cache, "no cache returned");

			// add the item
			cache.Put(key, value);
			Thread.Sleep(1000);

			// make sure it's there
			object item = cache.Get(key);
			Assert.IsNotNull(item, "couldn't find item in cache");

			// clear the cache
			cache.Clear();

			// make sure we don't get an item
			item = cache.Get(key);
			Assert.IsNull(item, "item still exists in cache");
		}

		[Test]
		public void TestDefaultConstructor()
		{
			ICache cache = new SharedCacheClient();
			Assert.IsNotNull(cache);
		}

		[Test]
		public void TestEmptyProperties()
		{
			ICache cache = new SharedCacheClient("nunit", new Dictionary<string, string>());
			Assert.IsNotNull(cache);
		}

		[Test]
		public void TestNoPropertiesConstructor()
		{
			ICache cache = new SharedCacheClient("nunit");
			Assert.IsNotNull(cache);
		}

		[Test]
		public void TestNullKeyGet()
		{
			ICache cache = new SharedCacheClient();
			cache.Put("nunit", "value");
			Thread.Sleep(1000);
			object item = cache.Get(null);
			Assert.IsNull(item);
		}

		[Test]
		public void TestNullKeyPut()
		{
			ICache cache = new SharedCacheClient();
			Assert.Throws<ArgumentNullException>(() => cache.Put(null, null));
		}

		[Test]
		public void TestNullKeyRemove()
		{
			ICache cache = new SharedCacheClient();
			Assert.Throws<ArgumentNullException>(() => cache.Remove(null));
		}

		[Test]
		public void TestNullValuePut()
		{
			ICache cache = new SharedCacheClient();
			Assert.Throws<ArgumentNullException>(() => cache.Put("nunit", null));
		}

		[Test]
		public void TestPut()
		{
			string key = "key1";
			string value = "value";

			ICache cache = provider.BuildCache("nunit", props);
			Assert.IsNotNull(cache, "no cache returned");

			Assert.IsNull(cache.Get(key), "cache returned an item we didn't add !?!");

			cache.Put(key, value);
			Thread.Sleep(1000);
			object item = cache.Get(key);
			Assert.IsNotNull(item);
			Assert.AreEqual(value, item, "didn't return the item we added");
		}

		[Test]
		public void TestRegions()
		{
			string key = "key";
			ICache cache1 = provider.BuildCache("nunit1", props);
			ICache cache2 = provider.BuildCache("nunit2", props);
			string s1 = "test1";
			string s2 = "test2";
			cache1.Put(key, s1);
			cache2.Put(key, s2);
			Thread.Sleep(1000);
			object get1 = cache1.Get(key);
			object get2 = cache2.Get(key);
			Assert.IsFalse(get1 == get2);
		}

		[Test]
		public void TestRemove()
		{
			string key = "key1";
			string value = "value";

			ICache cache = provider.BuildCache("nunit", props);
			Assert.IsNotNull(cache, "no cache returned");

			// add the item
			cache.Put(key, value);
			Thread.Sleep(1000);

			// make sure it's there
			object item = cache.Get(key);
			Assert.IsNotNull(item, "item just added is not there");

			// remove it
			cache.Remove(key);

			// make sure it's not there
			item = cache.Get(key);
			Assert.IsNull(item, "item still exists in cache");
		}
	}
}