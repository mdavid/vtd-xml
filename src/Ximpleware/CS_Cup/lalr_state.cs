
namespace TUVienna.CS_CUP
{
using System;
using System.Collections;

/** This class represents a state in the LALR viable prefix recognition machine.
 *  A state consists of an LALR item Set and a Set of transitions to other 
 *  states under terminal and non-terminal symbols.  Each state represents
 *  a potential configuration of the parser.  If the item Set of a state 
 *  includes an item such as: <pre>
 *    [A ::= B * C d E , {a,b,c}]
 *  </pre> 
 *  this indicates that when the parser is in this state it is currently 
 *  looking for an A of the given form, has already seen the B, and would
 *  expect to see an a, b, or c after this sequence is complete.  Note that
 *  the parser is normally looking for several things at once (represented
 *  by several items).  In our example above, the state would also include
 *  items such as: <pre>
 *    [C ::= * X e Z, {d}]
 *    [X ::= * f, {e}]
 *  </pre> 
 *  to indicate that it was currently looking for a C followed by a d (which
 *  would be reduced into a C, matching the first symbol in our production 
 *  above), and the terminal f followed by e.<p>
 *
 *  At runtime, the parser uses a viable prefix recognition machine made up
 *  of these states to parse.  The parser has two operations, shift and reduce.
 *  In a shift, it consumes one Symbol and makes a transition to a new state.
 *  This corresponds to "moving the dot past" a terminal in one or more items
 *  in the state (these new shifted items will then be found in the state at
 *  the end of the transition).  For a reduce operation, the parser is 
 *  signifying that it is recognizing the RHS of some production.  To do this
 *  it first "backs up" by popping a stack of previously saved states.  It 
 *  pops off the same number of states as are found in the RHS of the 
 *  production.  This leaves the machine in the same state is was in when the
 *  parser first attempted to find the RHS.  From this state it makes a 
 *  transition based on the non-terminal on the LHS of the production.  This
 *  corresponds to placing the parse in a configuration equivalent to having 
 *  replaced all the symbols from the the input corresponding to the RHS with 
 *  the symbol on the LHS.
 *
 * @see     java_cup.lalr_item
 * @see     java_cup.lalr_item_set
 * @see     java_cup.lalr_transition
 * @version last updated: 7/3/96
 * @author  Frank Flannery
 * translated to C# 08.09.2003 by Samuel Imriska
 *  
 */

public class lalr_state {
  /*-----------------------------------------------------------*/
  /*--- Constructor(s) ----------------------------------------*/
  /*-----------------------------------------------------------*/
       
  /** Constructor for building a state from a Set of items.
   * @param itms the Set of items that makes up this state.
   */
  public lalr_state(lalr_item_set itms) 
   {
     /* don't allow null or duplicate item sets */
     if (itms == null)
       throw new internal_error(
	 "Attempt to construct an LALR state from a null item Set");

     if (find_state(itms) != null)
       throw new internal_error(
	 "Attempt to construct a duplicate LALR state");

     /* assign a unique index */
      _index = next_index++;

     /* store the items */
     _items = itms;

     /* add to the global collection, keyed with its item Set */
     _all.Add(_items,this);
   }

  /*-----------------------------------------------------------*/
  /*--- (Access to) Static (Class) Variables ------------------*/
  /*-----------------------------------------------------------*/

  /** Collection of all states. */
  protected static Hashtable _all = new Hashtable();

  /** Collection of all states. */
  public static IEnumerator all() {return _all.Values.GetEnumerator();}

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Indicate total number of states there are. */
  public static int number() {return _all.Count;}

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Hash table to find states by their kernels (i.e, the original, 
   *  unclosed, Set of items -- which uniquely define the state).  This table 
   *  stores state objects using (a copy of) their kernel item sets as keys. 
   */
  protected static Hashtable _all_kernels = new Hashtable();

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Find and return state with a given a kernel item Set (or null if not 
   *  found).  The kernel item Set is the subset of items that were used to
   *  originally create the state.  These items are formed by "shifting the
   *  dot" within items of other states that have a transition to this one.
   *  The remaining elements of this state's item Set are added during closure.
   * @param itms the kernel Set of the state we are looking for. 
   */
  public static lalr_state find_state(lalr_item_set itms)
    {
      if (itms == null) 
  	return null;
      else
  	return (lalr_state)_all[itms];
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Static counter for assigning unique state indexes. */
  protected static int next_index = 0;

  /*-----------------------------------------------------------*/
  /*--- (Access to) Instance Variables ------------------------*/
  /*-----------------------------------------------------------*/

  /** The item Set for this state. */
  protected lalr_item_set _items;

  /** The item Set for this state. */
  public lalr_item_set items() {return _items;}

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** List of transitions out of this state. */
  protected lalr_transition _transitions = null;

  /** List of transitions out of this state. */
  public lalr_transition transitions() {return _transitions;}

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Index of this state in the parse tables */
  protected int _index;

  /** Index of this state in the parse tables */
  public int index() {return _index;}

  /*-----------------------------------------------------------*/
  /*--- Static Methods ----------------------------------------*/
  /*-----------------------------------------------------------*/

  /** Helper routine for debugging -- produces a dump of the given state
    * onto System.out.
    */
  protected static void dump_state(lalr_state st) 
    {
      lalr_item_set itms;
      lalr_item itm;
      production_part part;

      if (st == null) 
	{
	  Console.WriteLine("NULL lalr_state");
	  return;
	}

      Console.WriteLine("lalr_state [" + st.index() + "] {");
      itms = st.items();
	  IEnumerator e = itms.all();
      while ( e.MoveNext() )
	{
	  itm = (lalr_item)e.Current;
	  Console.Write("  [");
	  Console.Write(itm.the_production().lhs().the_symbol().name());
	  Console.Write(" ::= ");
	  for (int i = 0; i<itm.the_production().rhs_length(); i++)
	    {
	      if (i == itm.dot_pos()) Console.Write("(*) ");
	      part = itm.the_production().rhs(i);
	      if (part.is_action()) 
		Console.Write("{action} ");
	      else
		Console.Write(((symbol_part)part).the_symbol().name() + " ");
	    }
	  if (itm.dot_at_end()) Console.Write("(*) ");
	  Console.WriteLine("]");
	}
      Console.WriteLine("}");
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Propagate lookahead sets through the constructed viable prefix 
   *  recognizer.  When the machine is constructed, each item that results
      in the creation of another such that its lookahead is included in the
      other's will have a propagate link Set up for it.  This allows additions
      to the lookahead of one item to be included in other items that it 
      was used to directly or indirectly create.
   */
  protected static void propagate_all_lookaheads() 
    {
      /* iterate across all states */
	  IEnumerator st = all();
      while ( st.MoveNext() )
	{
	  /* propagate lookaheads out of that state */
	  ((lalr_state)st.Current).propagate_lookaheads();
	}
    }

  /*-----------------------------------------------------------*/
  /*--- General Methods ---------------------------------------*/
  /*-----------------------------------------------------------*/

  /** Add a transition out of this state to another.
   * @param on_sym the symbol the transition is under.
   * @param to_st  the state the transition goes to.
   */
  public void add_transition(symbol on_sym, lalr_state to_st) 
    
    {
      lalr_transition trans;

      /* create a new transition object and put it in our list */
      trans = new lalr_transition(on_sym, to_st, _transitions);
      _transitions = trans;
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Build an LALR viable prefix recognition machine given a start 
   *  production.  This method operates by first building a start state
   *  from the start production (based on a single item with the dot at
   *  the beginning and EOF as expected lookahead).  Then for each state
   *  it attempts to extend the machine by creating transitions out of
   *  the state to new or existing states.  When considering extension
   *  from a state we make a transition on each symbol that appears before
   *  the dot in some item.  For example, if we have the items: <pre>
   *    [A ::= a b * X c, {d,e}]
   *    [B ::= a b * X d, {a,b}]
   *  </pre>
   *  in some state, then we would be making a transition under X to a new
   *  state.  This new state would be formed by a "kernel" of items 
   *  corresponding to moving the dot past the X.  In this case: <pre>
   *    [A ::= a b X * c, {d,e}]
   *    [B ::= a b X * Y, {a,b}]
   *  </pre>
   *  The full state would then be formed by "closing" this kernel Set of 
   *  items so that it included items that represented productions of things
   *  the parser was now looking for.  In this case we would items 
   *  corresponding to productions of Y, since various forms of Y are expected
   *  next when in this state (see lalr_item_set.compute_closure() for details 
   *  on closure). <p>
   *
   *  The process of building the viable prefix recognizer terminates when no
   *  new states can be added.  However, in order to build a smaller number of
   *  states (i.e., corresponding to LALR rather than canonical LR) the state 
   *  building process does not maintain full loookaheads in all items.  
   *  Consequently, after the machine is built, we go back and propagate 
   *  lookaheads through the constructed machine using a call to 
   *  propagate_all_lookaheads().  This makes use of propagation links 
   *  constructed during the closure and transition process.
   *
   * @param start_prod the start production of the grammar
   * @see   java_cup.lalr_item_set#compute_closure
   * @see   java_cup.lalr_state#propagate_all_lookaheads
   */

  public static lalr_state build_machine(production start_prod) 
    
    {
      lalr_state    start_state;
      lalr_item_set start_items;
      lalr_item_set new_items;
      lalr_item_set linked_items;
      lalr_item_set kernel;
      Stack         work_stack = new Stack();
      lalr_state    st, new_st;
      symbol_set    outgoing;
      lalr_item     itm, new_itm, existing, fix_itm;
      symbol        sym, sym2;
      IEnumerator   i, s, fix;

      /* sanity check */
      if (start_prod == null)
	throw new internal_error(
 	  "Attempt to build viable prefix recognizer using a null production");

      /* build item with dot at front of start production and EOF lookahead */
      start_items = new lalr_item_set();

      itm = new lalr_item(start_prod);
      itm.lookahead().add(terminal.EOF);

      start_items.add(itm);

      /* create copy the item Set to form the kernel */
      kernel = new lalr_item_set(start_items);

      /* create the closure from that item Set */
      start_items.compute_closure();

      /* build a state out of that item Set and put it in our work Set */
      start_state = new lalr_state(start_items);
      work_stack.Push(start_state);

      /* enter the state using the kernel as the key */
      _all_kernels.Add(kernel, start_state);

      /* continue looking at new states until we have no more work to do */
      while (work_stack.Count!=0)
	{
	  /* remove a state from the work Set */
	  st = (lalr_state)work_stack.Pop();

	  /* gather up all the symbols that appear before dots */
	  outgoing = new symbol_set();
	  i = st.items().all();
	  while ( i.MoveNext() )
	    {
	      itm = (lalr_item)i.Current;

	      /* add the symbol before the dot (if any) to our collection */
	      sym = itm.symbol_after_dot();
	      if (sym != null) outgoing.add(sym);
	    }

	  /* now create a transition out for each individual symbol */
	  s = outgoing.all();
	  while ( s.MoveNext())
	    {
	      sym = (symbol)s.Current;

	      /* will be keeping the Set of items with propagate links */
	      linked_items = new lalr_item_set();

	      /* gather up shifted versions of all the items that have this
		 symbol before the dot */
	      new_items = new lalr_item_set();
		  i = st.items().all();
	      while ( i.MoveNext())
		{
		  itm = (lalr_item)i.Current;

		  /* if this is the symbol we are working on now, add to Set */
		  sym2 = itm.symbol_after_dot();
		  if (sym.Equals(sym2))
		    {
		      /* add to the kernel of the new state */
		      new_items.add(itm.shift());

		      /* remember that itm has propagate link to it */
		      linked_items.add(itm);
		    }
		}

	      /* use new items as state kernel */
	      kernel = new lalr_item_set(new_items);

	      /* have we seen this one already? */
	      new_st = (lalr_state)_all_kernels[kernel];

	      /* if we haven't, build a new state out of the item Set */
	      if (new_st == null)
		{
	          /* compute closure of the kernel for the full item Set */
	          new_items.compute_closure();

		  /* build the new state */
		  new_st = new lalr_state(new_items);

		  /* add the new state to our work Set */
		  work_stack.Push(new_st);

		  /* put it in our kernel table */
		  _all_kernels.Add(kernel, new_st);
		}
	      /* otherwise relink propagation to items in existing state */
	      else 
		{
		  /* walk through the items that have links to the new state */
		  fix = linked_items.all();
		  while ( fix.MoveNext() )
		    {
		      fix_itm = (lalr_item)fix.Current;

		      /* look at each propagate link out of that item */

		      for (int l =0; l < fix_itm.propagate_items().Count; l++)
			{
			  /* pull out item linked to in the new state */
			  new_itm =(lalr_item) fix_itm.propagate_items().ToArray()[l];

			  /* find corresponding item in the existing state */
			  existing = new_st.items().find(new_itm);

			  /* fix up the item so it points to the existing Set */
			
			  if (existing != null)
			  {
				  fix_itm.set_propagate_item(existing,l);

			  }
			}
		    }
		}

	      /* add a transition from current state to that state */
	      st.add_transition(sym, new_st);
	    }
	}

      /* all done building states */

      /* propagate complete lookahead sets throughout the states */
      propagate_all_lookaheads();

      return start_state;
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Propagate lookahead sets out of this state. This recursively 
   *  propagates to all items that have propagation links from some item 
   *  in this state. 
   */
  protected void propagate_lookaheads() 
    {
      /* recursively propagate out from each item in the state */
	  IEnumerator itm = items().all(); 
      while (itm.MoveNext())
	((lalr_item)itm.Current).propagate_lookaheads(null);
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Fill in the parse table entries for this state.  There are two 
   *  parse tables that encode the viable prefix recognition machine, an 
   *  action table and a reduce-goto table.  The rows in each table 
   *  correspond to states of the machine.  The columns of the action table
   *  are indexed by terminal symbols and correspond to either transitions 
   *  out of the state (shift entries) or reductions from the state to some
   *  previous state saved on the stack (reduce entries).  All entries in the
   *  action table that are not shifts or reduces, represent errors.    The
   *  reduce-goto table is indexed by non terminals and represents transitions 
   *  out of a state on that non-terminal.<p>
   *  Conflicts occur if more than one action needs to go in one entry of the
   *  action table (this cannot happen with the reduce-goto table).  Conflicts
   *  are resolved by always shifting for shift/reduce conflicts and choosing
   *  the lowest numbered production (hence the one that appeared first in
   *  the specification) in reduce/reduce conflicts.  All conflicts are 
   *  reported and if more conflicts are detected than were declared by the
   *  user, code generation is aborted.
   *
   * @param act_table    the action table to put entries in.
   * @param reduce_table the reduce-goto table to put entries in.
   */
  public void build_table_entries(
    parse_action_table act_table, 
    parse_reduce_table reduce_table)
    
    {
      parse_action_row our_act_row;
      parse_reduce_row our_red_row;
      lalr_item        itm;
      parse_action     act, other_act;
      symbol           sym;
      terminal_set     conflict_set = new terminal_set();

      /* pull out our rows from the tables */
      our_act_row = act_table.under_state[index()];
      our_red_row = reduce_table.under_state[index()];

      /* consider each item in our state */
	  IEnumerator i = items().all();
      while ( i.MoveNext() )
	{
	  itm = (lalr_item)i.Current;
	 

	  /* if its completed (dot at end) then reduce under the lookahead */
	  if (itm.dot_at_end())
	    {
	      act = new reduce_action(itm.the_production());

	      /* consider each lookahead symbol */
	      for (int t = 0; t < terminal.number(); t++)
		{
		  /* skip over the ones not in the lookahead */
		  if (!itm.lookahead().contains(t)) continue;

	          /* if we don't already have an action put this one in */
	          if (our_act_row.under_term[t].kind() == 
		      parse_action.ERROR)
		    {
	              our_act_row.under_term[t] = act;
		    }
	          else
		    {
		      /* we now have at least one conflict */
		      terminal term = terminal.find(t);
		      other_act = our_act_row.under_term[t];

		      /* if the other act was not a shift */
		      if ((other_act.kind() != parse_action.SHIFT) && 
			  (other_act.kind() != parse_action.NONASSOC))
		        {
		        /* if we have lower index hence priority, replace it*/
		          if (itm.the_production().index() < 
			      ((reduce_action)other_act).reduce_with().index())
			    {
			      /* replace the action */
			      our_act_row.under_term[t] = act;
			    }
		        } else {
			  /*  Check precedences,see if problem is correctable */
			  if(fix_with_precedence(itm.the_production(), 
						 t, our_act_row, act)) {
			    term = null;
			  }
			}
		      if(term!=null) {

			conflict_set.add(term);
		      }
		    }
		}
	    }
	}

      /* consider each outgoing transition */
      for (lalr_transition trans=transitions(); trans!=null; trans=trans.next())
	{
	  /* if its on an terminal add a shift entry */
	  sym = trans.on_symbol();
	  if (!sym.is_non_term())
	    {
	      act = new shift_action(trans.to_state());

	      /* if we don't already have an action put this one in */
	      if ( our_act_row.under_term[sym.index()].kind() == 
		   parse_action.ERROR)
		{
	          our_act_row.under_term[sym.index()] = act;
		}
	      else
		{
		  /* we now have at least one conflict */
		  production p = ((reduce_action)our_act_row.under_term[sym.index()]).reduce_with();

		  /* shift always wins */
		  if (!fix_with_precedence(p, sym.index(), our_act_row, act)) {
		    our_act_row.under_term[sym.index()] = act;
		    conflict_set.add(terminal.find(sym.index()));
		  }
		}
	    }
	  else
	    {
	      /* for non terminals add an entry to the reduce-goto table */
	      our_red_row.under_non_term[sym.index()] = trans.to_state();
	    }
	}

      /* if we end up with conflict(s), report them */
      if (!conflict_set.empty())
        report_conflicts(conflict_set);
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

    
  /** Procedure that attempts to fix a shift/reduce error by using
   * precedences.  --frankf 6/26/96
   *  
   *  if a production (also called rule) or the lookahead terminal
   *  has a precedence, then the table can be fixed.  if the rule
   *  has greater precedence than the terminal, a reduce by that rule
   *  in inserted in the table.  If the terminal has a higher precedence, 
   *  it is shifted.  if they have equal precedence, then the associativity
   *  of the precedence is used to determine what to put in the table:
   *  if the precedence is left associative, the action is to reduce. 
   *  if the precedence is right associative, the action is to shift.
   *  if the precedence is non associative, then it is a syntax error.
   *
   *  @param p           the production
   *  @param term_index  the index of the lokahead terminal
   *  @param parse_action_row  a row of the action table
   *  @param act         the rule in conflict with the table entry
   */

    protected bool fix_with_precedence(
		        production       p,
			int              term_index,
			parse_action_row table_row,
			parse_action     act)

       {

      terminal term = terminal.find(term_index);

      /* if the production has a precedence number, it can be fixed */
      if (p.precedence_num() > assoc.no_prec) {

	/* if production precedes terminal, put reduce in table */
	if (p.precedence_num() > term.precedence_num()) {
	  table_row.under_term[term_index] = 
	    insert_reduce(table_row.under_term[term_index],act);
	  return true;
	} 

	/* if terminal precedes rule, put shift in table */
	else if (p.precedence_num() < term.precedence_num()) {
	  table_row.under_term[term_index] = 
	    insert_shift(table_row.under_term[term_index],act);
	  return true;
	} 
	else {  /* they are == precedence */

	  /* equal precedences have equal sides, so only need to 
	     look at one: if it is right, put shift in table */
	  if (term.precedence_side() == assoc.right) {
	  table_row.under_term[term_index] = 
	    insert_shift(table_row.under_term[term_index],act);
	    return true;
	  }

	  /* if it is left, put reduce in table */
	  else if (term.precedence_side() == assoc.left) {
	    table_row.under_term[term_index] = 
	      insert_reduce(table_row.under_term[term_index],act);
	    return true;
	  }

	  /* if it is nonassoc, we're not allowed to have two nonassocs
	     of equal precedence in a row, so put in NONASSOC */
	  else if (term.precedence_side() == assoc.nonassoc) {
            table_row.under_term[term_index] = new nonassoc_action();
	    return true;
	  } else {
	    /* something really went wrong */
	    throw new internal_error("Unable to resolve conflict correctly");
	  }
	}
      }
      /* check if terminal has precedence, if so, shift, since 
	 rule does not have precedence */
      else if (term.precedence_num() > assoc.no_prec) {
	 table_row.under_term[term_index] = 
	   insert_shift(table_row.under_term[term_index],act);
	 return true;
      }
       
      /* otherwise, neither the rule nor the terminal has a precedence,
	 so it can't be fixed. */
      return false;
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/


  /*  given two actions, and an action type, return the 
      action of that action type.  give an error if they are of
      the same action, because that should never have tried
      to be fixed 
     
  */
    protected parse_action insert_action(
					parse_action a1,
					parse_action a2,
					int act_type) 
      
    {
      if ((a1.kind() == act_type) && (a2.kind() == act_type)) {
	throw new internal_error("Conflict resolution of bogus actions");
      } else if (a1.kind() == act_type) {
	return a1;
      } else if (a2.kind() == act_type) {
	return a2;
      } else {
	throw new internal_error("Conflict resolution of bogus actions");
      }
    }

    /* find the shift in the two actions */
    protected parse_action insert_shift(
					parse_action a1,
					parse_action a2) 
        
    {
      return insert_action(a1, a2, parse_action.SHIFT);
    }

    /* find the reduce in the two actions */
    protected parse_action insert_reduce(
					parse_action a1,
					parse_action a2) 
      
    {
      return insert_action(a1, a2, parse_action.REDUCE);
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Produce warning messages for all conflicts found in this state.  */
  protected void report_conflicts(terminal_set conflict_set)
    
    {
      lalr_item    itm, compare;

      bool      after_itm;

      /* consider each element */
	  IEnumerator itms = items().all();
      while ( itms.MoveNext() )
	{
	  itm = (lalr_item)itms.Current;

	  /* clear the S/R conflict Set for this item */

	  /* if it results in a reduce, it could be a conflict */
	  if (itm.dot_at_end())
	    {
	      /* not yet after itm */
	      after_itm = false;

	      /* compare this item against all others looking for conflicts */
		  IEnumerator comps = items().all(); 
	      while (comps.MoveNext() )
		{
		  compare = (lalr_item)comps.Current;

		  /* if this is the item, next one is after it */
		  if (itm == compare) after_itm = true;

		  /* only look at it if its not the same item */
		  if (itm != compare)
		    {
		      /* is it a reduce */
		      if (compare.dot_at_end())
			{
			  /* only look at reduces after itm */
			  if (after_itm)
                            /* does the comparison item conflict? */
                            if (compare.lookahead().intersects(itm.lookahead()))
                              /* report a reduce/reduce conflict */
                              report_reduce_reduce(itm, compare);
			}
		    }
		}
	      /* report S/R conflicts under all the symbols we conflict under */
	      for (int t = 0; t < terminal.number(); t++)
		if (conflict_set.contains(t))
		  report_shift_reduce(itm,t);
	    }
	}
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Produce a warning message for one reduce/reduce conflict. 
   *
   * @param itm1 first item in conflict.
   * @param itm2 second item in conflict.
   */
  protected void report_reduce_reduce(lalr_item itm1, lalr_item itm2)
    
    {
      bool comma_flag = false;

      System.Console.Error.WriteLine("*** Reduce/Reduce conflict found in state #"+index());
      System.Console.Error.Write  ("  between ");
      System.Console.Error.WriteLine(itm1.to_simple_string());
      System.Console.Error.Write  ("  and     ");
      System.Console.Error.WriteLine(itm2.to_simple_string());
      System.Console.Error.Write("  under symbols: {" );
      for (int t = 0; t < terminal.number(); t++)
	{
	  if (itm1.lookahead().contains(t) && itm2.lookahead().contains(t))
	    {
	      if (comma_flag) System.Console.Error.Write(", "); else comma_flag = true;
	      System.Console.Error.Write(terminal.find(t).name());
	    }
	}
      System.Console.Error.WriteLine("}");
      System.Console.Error.Write("  Resolved in favor of ");
      if (itm1.the_production().index() < itm2.the_production().index())
	System.Console.Error.WriteLine("the first production.\n");
      else
	System.Console.Error.WriteLine("the second production.\n");

      /* count the conflict */
      emit.num_conflicts++;
      lexer.warning_count++;
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Produce a warning message for one shift/reduce conflict.
   *
   * @param red_itm      the item with the reduce.
   * @param conflict_sym the index of the symbol conflict occurs under.
   */
  protected void report_shift_reduce(
    lalr_item red_itm, 
    int       conflict_sym)
    
    {
      lalr_item    itm;
      symbol       shift_sym;

      /* emit top part of message including the reduce item */
      System.Console.Error.WriteLine("*** Shift/Reduce conflict found in state #"+index());
      System.Console.Error.Write  ("  between ");
      System.Console.Error.WriteLine(red_itm.to_simple_string());

      /* find and report on all items that shift under our conflict symbol */
	  IEnumerator itms = items().all();
      while ( itms.MoveNext() )
	{
	  itm = (lalr_item)itms.Current;

	  /* only look if its not the same item and not a reduce */
	  if (itm != red_itm && !itm.dot_at_end())
	    {
	      /* is it a shift on our conflicting terminal */
	      shift_sym = itm.symbol_after_dot();
	      if (!shift_sym.is_non_term() && shift_sym.index() == conflict_sym)
	        {
		  /* yes, report on it */
                  System.Console.Error.WriteLine("  and     " + itm.to_simple_string());
		}
	    }
	}
      System.Console.Error.WriteLine("  under symbol "+ terminal.find(conflict_sym).name());
      System.Console.Error.WriteLine("  Resolved in favor of shifting.\n");

      /* count the conflict */
      emit.num_conflicts++;
      lexer.warning_count++;
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Equality comparison. */
  public bool Equals(lalr_state other)
    {
      /* we are equal if our item sets are equal */
      return other != null && items().Equals(other.items());
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Generic equality comparison. */
  public override bool Equals(object other)
    {
      if (other.GetType()!=typeof(lalr_state))
	return false;
      else
	return Equals((lalr_state)other);
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Produce a hash code. */
  public override int GetHashCode()
    {
      /* just use the item Set hash code */
      return items().GetHashCode();
    }

  /*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/

  /** Convert to a string. */
  public override string ToString()
    {
      string result;
      lalr_transition tr;

      /* dump the item Set */
      result = "lalr_state [" + index() + "]: " + _items + "\n";

      /* do the transitions */
      for (tr = transitions(); tr != null; tr = tr.next())
	{
	  result += tr;
	  result += "\n";
	}

      return result;
    }

  /*-----------------------------------------------------------*/
}
}