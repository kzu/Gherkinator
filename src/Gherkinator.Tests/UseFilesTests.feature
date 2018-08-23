Feature: UseFiles
  An extension that allows assigning and verifying file contents

  Scenario: Setting file contents inline
    Given Foo.txt = Bar
    Then Foo.txt = "Bar"

  Scenario: Setting file contents block
    Given Foo.txt = 
"""
Bar
"""
    Then Foo.txt = Bar

  Scenario: Verifying file contents
    Given Foo.txt = Bar
    # This Then will fail, but the test is asserting the failure
    Then Foo.txt = Baz (will fail)

  Scenario: Disposing deletes temporary directory
    # The verification happens in the test itself because 
    # it must check *after* the scenario is run
    Given Foo.txt = Bar